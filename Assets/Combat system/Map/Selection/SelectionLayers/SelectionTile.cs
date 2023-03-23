using System;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    public class SelectionTile : SelectionLayer
    {
        public List<Vector2Int> Positions;
        public List<int> PreviousVector;
        
        public Vector2Int Origin;
        
        public readonly SelectionTileFilter Filter;
        private readonly Action<SelectionTile, Vector2Int> OnSelected;
        private readonly Action<SelectionTile> OnCancel;
        private readonly Action<SelectionTile, Vector2Int?> OnHover;
        public readonly int Size;

        public SelectionTile(Vector2Int Origin,
            SelectionTileFilter Filter,
            int Size,
            Action<SelectionTile, Vector2Int> OnSelected = null,
            Action<SelectionTile> OnCancel = null,
            Action<SelectionTile, Vector2Int?> OnHover = null)
        {
            Positions = new();
            if (Filter.NeedPath)
                PreviousVector = new();
            else
                PreviousVector = null;

            this.Origin = Origin;
            this.Filter = Filter;
            this.OnSelected = OnSelected;
            this.OnCancel = OnCancel;
            this.Size = Size;
            this.OnHover = OnHover;

            ApplySelectionFilter();
        }

        private void ApplySelectionFilter()
        {
            Positions.Clear();

            if (Filter.NeedPath)
                ApplySelectionFilterContinuous();
            else
                ApplySelectionFilterNonContinuous();
        }

        private void ApplySelectionFilterNonContinuous()
        {
            for (int i = 0; i < BattleMap.Tiles.Count; i++)
                ApplyFilterToDelta(i);
        }

        private bool ApplyFilterToDelta(int delta)
        {
            var pos = BattleMap.DeltaToPos(delta);
            var AddToPos = Filter.Filter(this, pos, out bool block);

            if (AddToPos)
                Positions.Add(pos);
            
            return !block;
        }

        private void ApplySelectionFilterContinuous()
        {
            for (int i = 0; i < BattleMap.Tiles.Count; i++)
                PreviousVector.Add(-1);
            
            Dictionary<int, int> Visible = new(PreviousVector.Count);
            Dictionary<int, int> NextVisible = new(PreviousVector.Count);
            var deltaOrigin = BattleMap.PosToDelta(Origin);
            Visible.Add(deltaOrigin, deltaOrigin);

            while (Visible.Count > 0)
            {
                NextVisible.Clear();
                foreach (var pair in Visible)
                {
                    var prev = pair.Value;
                    var current = pair.Key;

                    if (PreviousVector[current] != -1)
                        continue;

                    PreviousVector[current] = prev;

                    if (!ApplyFilterToDelta(current))
                        continue;
                    
                    var pos = BattleMap.DeltaToPos(current);

                    for (int i = 0; i < 4; i++)
                    {
                        var x = i % 2 == 0 ? i - 1 : 0;
                        var y = i % 2 == 0 ? 0 : i - 2;

                        var next = BattleMap.PosToDelta(pos + new Vector2Int(x, y));

                        if (next == -1)
                            continue;

                        if (!Visible.ContainsKey(next))
                            NextVisible[next] = current;
                    }
                }

                Visible.Clear();
                foreach (var pair in NextVisible)
                    Visible.Add(pair.Key, pair.Value);
            }
        }
        
        public int CountPath(Vector2Int position)
        {
            int count = 0;
            int current = BattleMap.PosToDelta(position);
            int prev = PreviousVector[current];
            while (current != prev)
            {
                current = prev;
                prev = PreviousVector[prev];
                count++;
            }
            return count;
        }

        public void Select(Vector2Int Position) => OnSelected?.Invoke(this, Position);
        public void Cancel() => OnCancel?.Invoke(this);
        public void Hover(Vector2Int? Position) => OnHover?.Invoke(this, Position);
    }
}