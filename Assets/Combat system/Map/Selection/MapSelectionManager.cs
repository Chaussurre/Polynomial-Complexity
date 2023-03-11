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
        public int Hovered { get; private set; } = -1;
        public TileSelector HoveredTile => Hovered != -1? Tiles[Hovered] : null;

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
            Action<Vector2Int, MapSelectionManager> OnSelected, Action<MapSelectionManager> OnCancel, int Size,
            Action<MapSelectionManager, Vector2Int?> ReColor = null)
        {
            SelectionLayers.Push(new SelectionLayer(Origin, 
                Filter, 
                OnSelected, 
                OnCancel, 
                Size, 
                BattleMap, 
                ReColor));

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

        void UnHover()
        {
            foreach (var tile in Tiles)
                tile.SetColor(null);

            if (Hovered == -1) return;
            
            var pos = BattleMap.DeltaToPos(Hovered);
            for (int x = 0; x < BattleMap.Size[0]; x++)
                Tiles[BattleMap.PosToDelta(new Vector2Int(x, pos.y))].Refresh();
            for (int y = 0; y < BattleMap.Size[1]; y++)
                Tiles[BattleMap.PosToDelta(new Vector2Int(pos.x, y))].Refresh();
        }
        
        void CheckHover(Vector2 mousePos)
        {
            UnHover();
            
            var index = Tiles.FindIndex(selector => selector.isOverMe(mousePos));
            Hovered = index;

            if (SelectionLayers.Count > 0)
            {
                var Layer = LastLayer;
                Layer.OnHover?.Invoke(this, Hovered == -1 ? null :  BattleMap.DeltaToPos(Hovered));
            }
            
            if (index == -1) return;
            
            var pos = BattleMap.DeltaToPos(index);

            var selector = Tiles[index];

            for (int x = 0; x < BattleMap.Size[0]; x++)
                Tiles[BattleMap.PosToDelta(new Vector2Int(x, pos.y))].HalfHover();
            for (int y = 0; y < BattleMap.Size[1]; y++)
                Tiles[BattleMap.PosToDelta(new Vector2Int(pos.x, y))].HalfHover();
            selector.Hover();
        }

        private void Update()
        {
            var cam = Camera.main;
            
            if(!cam) return;
            var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            
            if (Hovered == -1 || !HoveredTile.isOverMe(mousePos))
                CheckHover(mousePos);

            if (Input.GetMouseButtonDown(0))
                Select(mousePos);

            if (Input.GetMouseButtonDown(1))
                Cancel(mousePos);
        }

        void Select(Vector2 MousePos)
        {
            if (SelectionLayers.Count == 0)
            {
                var pos = GetSelectorPos(HoveredTile);
                if (BattleMap.CombatEntitiesPos.TryGetValue(pos, out var entity))
                    entity.MovementManager?.SelectMove(this, pos);
            }
            else
            {
                var layer = LastLayer;

                foreach (var position in layer.Positions) Tiles[BattleMap.PosToDelta(position)].Refresh();

                var pos = GetSelectorPos(HoveredTile);
                if (layer.Positions.Contains(pos))
                    layer.OnSelected?.Invoke(pos, this);
            }

            //Put hovered tile back in hover mode
            CheckHover(MousePos);
        }

        void Cancel(Vector2 MousePos)
        {
            if (SelectionLayers.TryPop(out var layer))
                layer.OnCancel?.Invoke(this);
            
            RefreshSelectionFilter();

            //Put hovered tile back in hover mode
            CheckHover(MousePos);
        }
    }
}
