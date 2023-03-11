using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    public struct SelectionLayer
    {
        public List<Vector2Int> Positions;
        public List<int> PreviousVector;
        
        public Vector2Int Origin;
        public SelectionLayerFilter Filter;
        public Action<Vector2Int, MapSelectionManager> OnSelected;
        public Action<MapSelectionManager> OnCancel;
        public Action<MapSelectionManager, Vector2Int?> OnHover;
        public int Size;
        public BattleMap Map;

        public SelectionLayer(Vector2Int Origin,
            SelectionLayerFilter Filter,
            Action<Vector2Int, MapSelectionManager> OnSelected,
            Action<MapSelectionManager> OnCancel,
            int Size,
            BattleMap Map, Action<MapSelectionManager, Vector2Int?> OnHover)
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
            this.Map = Map;
            this.OnHover = OnHover;
            
            ApplySelectionFilter();
        }
        
        void ApplySelectionFilter()
        {
            Positions.Clear();

            if (Filter.NeedPath)
                ApplySelectionFilterContinuous();
            else
                ApplySelectionFilterNonContinuous();
        }

        void ApplySelectionFilterNonContinuous()
        {
            for (int i = 0; i < Map.Tiles.Count; i++)
                ApplyFilterToDelta(i);
        }

        private bool ApplyFilterToDelta(int delta)
        {
            var pos = Map.DeltaToPos(delta);
            var AddToPos = Filter.Filter(this, pos, out bool block);

            if (AddToPos)
                Positions.Add(pos);
            
            return !block;
        }

        void ApplySelectionFilterContinuous()
        {
            for (int i = 0; i < Map.Tiles.Count; i++)
                PreviousVector.Add(-1);


            Dictionary<int, int> Visible = new(PreviousVector.Count);
            Dictionary<int, int> NextVisible = new(PreviousVector.Count);
            var deltaOrigin = Map.PosToDelta(Origin);
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
                    
                    var pos = Map.DeltaToPos(current);

                    for (int i = 0; i < 4; i++)
                    {
                        var x = i % 2 == 0 ? i - 1 : 0;
                        var y = i % 2 == 0 ? 0 : i - 2;

                        var next = Map.PosToDelta(pos + new Vector2Int(x, y));

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
            int current = Map.PosToDelta(position);
            int prev = PreviousVector[current];
            while (current != prev)
            {
                current = prev;
                prev = PreviousVector[prev];
                count++;
            }
            return count;
        }
    }
}