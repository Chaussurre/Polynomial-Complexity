using System.Collections.Generic;
using CombatSystem.Actions;
using CombatSystem.Map;
using CombatSystem.Selection;
using TMPro;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class MovementManager : ActionManager
    {
        public SelectionTileFilter MovementFilter;

        public int Speed;
        private int RemainingMoves;
        
        [SerializeField] private Color NextPathColor = Color.white;
        [SerializeField] private Color PreviousPathColor = Color.white;
        [SerializeField] private float MoveTimePerTile = 2f;
        [SerializeField] private string AnimationTriggerName;
        
        private CombatEntity Entity;

        private struct PathStep
        {
            public int TargetDelta;
            public int Cost;
            public bool Choice;
        }
        
        private readonly Stack<PathStep> Path = new();

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
        }

        private void OnSelect(SelectionTile selectionTile, Vector2Int position)
        {

            if (position == selectionTile.Origin)
                FinishMove(position);
            else
            {
                DoPath(selectionTile, position);
                SelectMoveWithSpeed(position, MovementFilter.AllowReChoice ? RemainingMoves : 0);
            }
        }

        private void OnCancel(SelectionTile tile)
        {
            //popping Path until we reach the last position a choice has been made
            if(Path.Count == 0) return;

            var count = 0;
            bool end; 
            do
            {
                var step = Path.Pop();
                end = step.Choice;
                RemainingMoves += step.Cost;
                count++;
                
            } while (!end && Path.Count > 0);

            MapActionBuffer.PopActions.Invoke(count);
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
            
            foreach (var step in Path) Tiles[step.TargetDelta].TileSelector.SetColor(PreviousPathColor);

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
            SelectionStackManager.AddLayer.Invoke(new SelectionTile(
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

        private void DoPath(SelectionTile selection, Vector2Int position)
        {
            var delta = BattleMap.PosToDelta(position);
            
            if (selection.Filter.NeedPath)
                GetPath(selection, delta);
            else
            {
                Path.Push(new PathStep
                {
                    TargetDelta = delta,
                    Cost = RemainingMoves,
                    Choice = true
                });
                AddMoveAction(selection.Origin, position, true);
                RemainingMoves = 0; 
            }
        }

        private void GetPath(SelectionTile selection, int delta)
        {
            void Recurse_GetPath(int _delta)
            {
                var prev = selection.PreviousVector[_delta];
                var end = prev == _delta;
                if (!end)
                {
                    Recurse_GetPath(prev);
                    AddMoveAction(prev, _delta);
                }

                Path.Push(new PathStep
                {
                    TargetDelta = _delta,
                    Cost = 1,
                    Choice = end
                });
                RemainingMoves--;
            }

            var prev = selection.PreviousVector[delta];
            Recurse_GetPath(prev);
            AddMoveAction(prev, delta);
        }

        private void AddMoveAction(int from, int to, bool teleport = false)
        {
            AddMoveAction(BattleMap.DeltaToPos(from), BattleMap.DeltaToPos(to), teleport);
        }

        private void AddMoveAction(Vector2Int from, Vector2Int to, bool teleport = false)
        {
            var action = new MapActionMovement(Entity, from, to);
            action.CreateView(teleport? 0: MoveTimePerTile, AnimationTriggerName);
            
            MapActionBuffer.AddAction.Invoke(action);
        }
    }
}
