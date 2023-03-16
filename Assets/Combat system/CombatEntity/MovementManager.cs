using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class MovementManager : MonoBehaviour
    {
        private CombatEntity Entity;
        public SelectionLayerFilter MovementFilter;

        public int Speed;
        public int RemainingMoves { get; protected set; }
        
        [SerializeField] private Color NextPathColor = Color.white;
        [SerializeField] private Color PreviousPathColor = Color.white;

        private Stack<int> Path = new();
        private Stack<int> MoveChoices = new();

        private void Start()
        {
            Entity = GetComponent<CombatEntity>();
            RemainingMoves = Speed;
        }

        public void SelectMove(Vector2Int Position)
        {
            int moves = MovementFilter.UseSpeed ? RemainingMoves : MovementFilter.DefaultSpeed;
            
            SelectMoveWithSpeed(Position, moves);
        }

        public void OnSelect(SelectionLayer Layer, Vector2Int position)
        {
            BattleMap.MoveEntity?.Invoke(Entity, position);
            var delta = BattleMap.PosToDelta(position);

            if (MovementFilter.AllowReChoice)
            {
                GetPath(Layer, delta);
                Path.TryPop(out _);
                
                var movesDone = Path.Count - (MoveChoices.Count > 0 ? MoveChoices.Peek() : 0);
                var movesLeft = Layer.Size - movesDone;
                
                MoveChoices.Push(Path.Count);

                if (movesDone <= 0)
                    FinishMove();
                else
                    SelectMoveWithSpeed(position, movesLeft);
            }
            else
            {
                Path.Push(BattleMap.PosToDelta(Layer.Origin));
                
                MoveChoices.Push(Path.Count);
                if (Layer.Origin == position)
                    FinishMove();
                else
                    SelectMoveWithSpeed(position, 0);
            }
        }

        public void OnCancel(SelectionLayer Layer)
        {
            if (Path.Count > 0)
            {
                MoveChoices.TryPop(out _);
                var lastChoice = MoveChoices.TryPeek(out var choice) ? choice : 0;

                while (Path.Count > lastChoice + 1)
                    Path.Pop();

                var position = BattleMap.DeltaToPos(Path.Pop());
                BattleMap.MoveEntity?.Invoke(Entity, position);
            }
        }

        void OnHover(SelectionLayer Layer, Vector2Int? Hovered)
        {
            var Tiles = BattleMap.Tiles;

            foreach (var tileDelta in Path) Tiles[tileDelta].TileSelector.SetColor(PreviousPathColor);

            if (!Hovered.HasValue || !Layer.Positions.Contains(Hovered.Value)) return;
            
            var delta = BattleMap.PosToDelta(Hovered.Value);
            var prev = Layer.PreviousVector[delta];
            do
            {
                Tiles[delta].TileSelector.SetColor(NextPathColor);

                delta = prev;
                prev = Layer.PreviousVector[delta];
            } while (delta != prev);

            Tiles[delta].TileSelector.SetColor(Color.blue);
        }

        void SelectMoveWithSpeed(Vector2Int Position, int Moves)
        {
            SelectionStackManager.AddLayer?.Invoke(new SelectionLayer(
                Position, 
                MovementFilter, 
                OnSelect, 
                OnCancel, 
                OnHover,
                Moves));
            //SelectionStackManager.AddLayer(Position, MovementFilter, OnSelect, OnCancel, Moves, OnHover);
        }

        void FinishMove()
        {
            MoveChoices.Clear();
            Path.Clear();
            
            SelectionStackManager.ClearStack?.Invoke();
        }

        private void GetPath(SelectionLayer Selection, int delta)
        {
            var prev = Selection.PreviousVector[delta];
            if (prev != delta)
                GetPath(Selection, prev);
            Path.Push(delta);
        }
    }
}
