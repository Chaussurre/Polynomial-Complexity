using System.Collections.Generic;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class MovementManager : MonoBehaviour
    {
        private CombatEntity Entity;
        public SelectionTileFilter MovementFilter;

        public int Speed;
        public int RemainingMoves { get; protected set; }
        
        [SerializeField] private Color NextPathColor = Color.white;
        [SerializeField] private Color PreviousPathColor = Color.white;

        private readonly Stack<int> Path = new();
        private readonly Stack<int> MoveChoices = new();

        private void Start()
        {
            Entity = GetComponent<CombatEntity>();
            RemainingMoves = Speed;
        }

        public void SelectMove(Vector2Int Position)
        {
            int moves = RemainingMoves;
            
            SelectMoveWithSpeed(Position, moves);
        }

        private void OnSelect(SelectionTile selectionTile, Vector2Int position)
        {
            BattleMap.MoveEntity?.Invoke(Entity, position);
            var delta = BattleMap.PosToDelta(position);

            if (MovementFilter.AllowReChoice)
            {
                GetPath(selectionTile, delta);
                Path.TryPop(out _);
                
                var movesDone = Path.Count - (MoveChoices.Count > 0 ? MoveChoices.Peek() : 0);
                var movesLeft = selectionTile.Size - movesDone;
                
                MoveChoices.Push(Path.Count);

                if (movesDone <= 0)
                    FinishMove(position);
                else
                    SelectMoveWithSpeed(position, movesLeft);
            }
            else
            {
                Path.Push(BattleMap.PosToDelta(selectionTile.Origin));
                
                MoveChoices.Push(Path.Count);
                if (selectionTile.Origin == position)
                    FinishMove(position);
                else
                    SelectMoveWithSpeed(position, 0);
            }
        }

        private void OnCancel(SelectionTile tile)
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

        private void OnHover(SelectionTile tile, Vector2Int? Hovered)
        {
            var Tiles = BattleMap.Tiles;

            if (!MovementFilter.NeedPath)
            {
                if (!Hovered.HasValue) return;
                
                Tiles[BattleMap.PosToDelta(Hovered.Value)]
                    .TileSelector
                    .SetColor(NextPathColor);
                Tiles[BattleMap.PosToDelta(tile.Origin)]
                    .TileSelector
                    .SetColor(NextPathColor);
                return;
            }
            
            foreach (var tileDelta in Path) Tiles[tileDelta].TileSelector.SetColor(PreviousPathColor);

            if (!Hovered.HasValue || !tile.Positions.Contains(Hovered.Value)) return;

            var delta = BattleMap.PosToDelta(Hovered.Value);
            var prev = tile.PreviousVector[delta];
            do
            {
                Tiles[delta].TileSelector.SetColor(NextPathColor);

                delta = prev;
                prev = tile.PreviousVector[delta];
            } while (delta != prev);

            Tiles[delta].TileSelector.SetColor(NextPathColor);
        }

        private void SelectMoveWithSpeed(Vector2Int Position, int Moves)
        {
            SelectionStackManager.AddLayer?.Invoke(new SelectionTile(
                Position, 
                MovementFilter, 
                Moves,
                OnSelect, 
                OnCancel, 
                OnHover));
        }

        private void FinishMove(Vector2Int FinalPosition)
        {
            MoveChoices.Clear();
            RemainingMoves -= Path.Count;
            Path.Clear();

            Entity.FinishMovement(FinalPosition);
        }

        private void GetPath(SelectionTile Selection, int delta)
        {
            var prev = Selection.PreviousVector[delta];
            if (prev != delta)
                GetPath(Selection, prev);
            Path.Push(delta);
        }
    }
}
