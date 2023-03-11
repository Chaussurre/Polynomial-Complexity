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

        public void SelectMove(MapSelectionManager MapSelection, Vector2Int Position)
        {
            int moves = MovementFilter.UseSpeed ? RemainingMoves : MovementFilter.DefaultSpeed;
            SelectMoveWithSpeed(MapSelection, Position, moves);
        }

        public void OnSelect(Vector2Int position, MapSelectionManager MapSelection)
        {
            var map = MapSelection.BattleMap;
            map.MoveCombatEntity(Entity, position);
            var delta = map.PosToDelta(position);
            var layer = MapSelection.LastLayer;

            if (MovementFilter.AllowReChoice)
            {
                GetPath(layer, delta);
                Path.TryPop(out _);
                
                var movesDone = Path.Count - (MoveChoices.Count > 0 ? MoveChoices.Peek() : 0);
                var movesLeft = layer.Size - movesDone;
                
                MoveChoices.Push(Path.Count);

                if (movesDone <= 0)
                    FinishMove(MapSelection);
                else
                    SelectMoveWithSpeed(MapSelection, position, movesLeft);
            }
            else
            {
                Path.Push(map.PosToDelta(layer.Origin));
                
                MoveChoices.Push(Path.Count);
                if (layer.Origin == position)
                    FinishMove(MapSelection);
                else
                    SelectMoveWithSpeed(MapSelection, position, 0);
            }
        }

        public void OnCancel(MapSelectionManager MapSelection)
        {
            if (Path.Count > 0)
            {
                MoveChoices.TryPop(out _);
                var lastChoice = MoveChoices.TryPeek(out var choice) ? choice : 0;

                while (Path.Count > lastChoice + 1)
                    Path.Pop();

                var position = MapSelection.BattleMap.DeltaToPos(Path.Pop());
                MapSelection.BattleMap.MoveCombatEntity(Entity, position);
            }
        }

        void SelectMoveWithSpeed(MapSelectionManager MapSelection, Vector2Int Position, int Moves)
        {
            MapSelection.AddSelectionLayer(Position, MovementFilter, OnSelect, OnCancel, Moves, Recolor);
        }

        void FinishMove(MapSelectionManager MapSelection)
        {
            MoveChoices.Clear();
            Path.Clear();
            
            MapSelection.EndStack();
        }

        void Recolor(MapSelectionManager mapSelectionManager, Vector2Int? Hovered)
        {
            var layer = mapSelectionManager.LastLayer;
            var bMap = mapSelectionManager.BattleMap;

            foreach (var tileDelta in Path) mapSelectionManager.Tiles[tileDelta].SetColor(PreviousPathColor);

            if (!Hovered.HasValue || !layer.Positions.Contains(Hovered.Value)) return;
            
            var delta = bMap.PosToDelta(Hovered.Value);
            var prev = layer.PreviousVector[delta];
            do
            {
                mapSelectionManager.Tiles[delta].SetColor(NextPathColor);

                delta = prev;
                prev = layer.PreviousVector[delta];
            } while (delta != prev);

            mapSelectionManager.Tiles[delta].SetColor(Color.blue);
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
