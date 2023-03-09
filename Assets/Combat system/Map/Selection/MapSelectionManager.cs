using System;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Map;
using UnityEngine;


namespace CombatSystem.Selection
{

    public class MapSelectionManager : MonoBehaviour
    {
        public BattleMap BattleMap;

        public readonly List<TileSelector> Tiles = new();
        public TileSelector Hovered { get; private set; }

        private readonly Stack<SelectionLayer> SelectionLayers = new();

        public List<int> PreviousVector;

        public SelectionLayer LastLayer => SelectionLayers.First();

        private void Start()
        {
            Tiles.AddRange(GetComponentsInChildren<TileSelector>());
            PreviousVector = new(Tiles.Count);
            for (int i = 0; i < Tiles.Count; i++)
                PreviousVector.Add(-1);
        }

        public Vector2Int GetSelectorPos(TileSelector selector)
        {
            return BattleMap.DeltaToPos(Tiles.IndexOf(selector));
        }

        public void AddSelectionLayer(Vector2Int Origin, SelectionLayerFilter Filter,
            Action<Vector2Int, MapSelectionManager> OnSelected, Action<MapSelectionManager> OnCancel, int Size)
        {
            SelectionLayers.Push(new SelectionLayer(Origin, Filter, OnSelected, OnCancel, Size, BattleMap));

            RefreshSelectionFilter();
        }

        public void EndStack()
        {
            SelectionLayers.Clear();
            foreach (var tile in Tiles)
                tile.SetState(TileSelectorState.NoCurrentLayer);
        }

        private void RefreshSelectionFilter()
        {
            if (SelectionLayers.Count == 0)
                EndStack();
            else
                for (int i = 0; i < Tiles.Count; i++)
                {
                    var pos = BattleMap.DeltaToPos(i);
                    Tiles[i].SetState(LastLayer.Positions.Contains(pos)
                        ? TileSelectorState.OnLayer
                        : TileSelectorState.OffLayer);
                }
        }

        void CheckHover(Vector2 mousePos)
        {
            Hovered = null;
            foreach (var selector in Tiles)
                if (selector.isOverMe(mousePos))
                {
                    Hovered = selector;
                    selector.Hover();
                }
        }

        private void Update()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!Hovered)
                CheckHover(mousePos);
            else if (!Hovered.isOverMe(mousePos))
            {
                Hovered.UnHover();
                CheckHover(mousePos);
            }

            if (Input.GetMouseButtonDown(0))
                Select();

            if (Input.GetMouseButtonDown(1))
                Cancel();
        }

        void Select()
        {
            if (SelectionLayers.Count == 0)
            {
                var pos = GetSelectorPos(Hovered);
                if (BattleMap.CombatEntitiesPos.TryGetValue(pos, out var entity))
                    entity.MovementManager?.SelectMove(this, pos);
            }
            else
            {
                var layer = LastLayer;
                var pos = GetSelectorPos(Hovered);
                if (layer.Positions.Contains(pos))
                    layer.OnSelected?.Invoke(pos, this);
            }

            //Put hovered tile back in hover mode
            Hovered?.Hover();
        }

        void Cancel()
        {
            if(SelectionLayers.TryPop(out var layer))
                layer.OnCancel?.Invoke(this);
            RefreshSelectionFilter();

            //Put hovered tile back in hover mode
            Hovered?.Hover();
        }
    }
}
