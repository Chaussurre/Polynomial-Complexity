using System.Collections.Generic;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class MovementManager : ActionManager
    {
        public SelectionTileFilter MovementFilter;

        public int Speed;
        public int RemainingMoves { get; protected set; }
        
        [SerializeField] private Color NextPathColor = Color.white;
        [SerializeField] private Color PreviousPathColor = Color.white;

        private CombatEntity Entity;
        private readonly Stack<int> Path = new();
        private readonly Stack<int> MoveChoices = new();
        private readonly Stack<int> MovesDone = new();

        public override string ID => "Movement";

        private void Start()
        {
            Entity = GetComponentInParent<CombatEntity>();
        }

        public override bool SelectAction(Vector2Int Position)
        {
            if (RemainingMoves == 0) return false;

            SelectMoveWithSpeed(Position, RemainingMoves);

            return true;
        }

        public override void ResetTurn()
        {
            RemainingMoves = Speed;
        }

        public override void EndTurn()
        {
            Path.Clear();
            MoveChoices.Clear();
            MovesDone.Clear();
        }

        private void OnSelect(SelectionTile selectionTile, Vector2Int position)
        {
            BattleMap.MoveEntity?.Invoke(Entity, position);
            var delta = BattleMap.PosToDelta(position);

            if (MovementFilter.NeedPath)
            {
                if (position == selectionTile.Origin)
                {
                    //MoveChoices.Push(Path.Count);
                    FinishMove(position);
                }
                else
                {
                    GetPath(selectionTile, delta);
                    Path.TryPop(out _);
                
                    if (MovementFilter.AllowReChoice)
                    {
                        var movesCount = Path.Count - (MoveChoices.Count > 0 ? MoveChoices.Peek() : 0);
                        MovesDone.Push(movesCount);
                        RemainingMoves -= movesCount;
                    }
                    else
                    {
                        MovesDone.Push(RemainingMoves);
                        RemainingMoves = 0;
                    }
                
                    MoveChoices.Push(Path.Count);
                    
                    SelectMoveWithSpeed(position, RemainingMoves);
                }
            }
            else
            {
                Path.Push(BattleMap.PosToDelta(selectionTile.Origin));
                
                MoveChoices.Push(Path.Count);
                MovesDone.Push(RemainingMoves);
                RemainingMoves = 0;
                
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

            if (MovesDone.TryPop(out var moves))
            {
                RemainingMoves += moves;
            }
        }

        private void OnHover(SelectionTile selectionTile, Vector2Int? Hovered)
        {
            var Tiles = BattleMap.Tiles;

            if (!MovementFilter.NeedPath)
            {
                if (!Hovered.HasValue) return;
                
                Tiles[BattleMap.PosToDelta(Hovered.Value)]
                    .TileSelector
                    .SetColor(NextPathColor);
                Tiles[BattleMap.PosToDelta(selectionTile.Origin)]
                    .TileSelector
                    .SetColor(NextPathColor);
                return;
            }
            
            foreach (var tileDelta in Path) Tiles[tileDelta].TileSelector.SetColor(PreviousPathColor);

            Tiles[BattleMap.PosToDelta(selectionTile.Origin)].TileSelector.SetColor(NextPathColor);
            
            if (!Hovered.HasValue || !selectionTile.Positions.Contains(Hovered.Value)) return;

            var delta = BattleMap.PosToDelta(Hovered.Value);
            var prev = selectionTile.PreviousVector[delta];
            do
            {
                Tiles[delta].TileSelector.SetColor(NextPathColor);

                delta = prev;
                prev = selectionTile.PreviousVector[delta];
            } while (delta != prev);
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

        private void FinishMove(Vector2Int position)
        {
            Entity.NextTurnStep(position);
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
