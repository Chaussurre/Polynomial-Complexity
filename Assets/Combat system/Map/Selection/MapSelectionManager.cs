using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;
using UnityEngine.Events;


namespace CombatSystem.Selection
{

    public class MapSelectionManager : MonoBehaviour
    {
        #region Events

        public static UnityEvent<Vector2Int> OnSelect = new();
        public static UnityEvent OnCancel = new();
        public static UnityEvent<Vector2Int?> OnHover = new();

        #endregion

        [SerializeField] protected Camera SelectionCamera;
        
        private int Hovered { get; set; } = -1;
        private TileSelector HoveredTile => Hovered != -1? GetTile(Hovered) : null;

        private void Awake()
        {
            SelectionStackManager.OnLayerBecomeActive.AddListener(RefreshSelectionFilter);            
        }

        private TileSelector GetTile(int index)
        {
            return BattleMap.Tiles[index].TileSelector;
        }

        private void RefreshSelectionFilter(SelectionLayer layer)
        {
            switch (layer)
            {
                case null:
                    ResetTilesState();
                    break;
                case SelectionTile tileLayer:
                    for (var i = 0; i < BattleMap.Tiles.Count; i++)
                        GetTile(i).SetState(tileLayer.Positions.Contains(BattleMap.DeltaToPos(i))
                            ? TileSelectorState.OnLayer
                            : TileSelectorState.OffLayer);
                    break;
                case SelectionAction tileAction:
                    for (var i = 0; i < BattleMap.Tiles.Count; i++)
                        GetTile(i).SetState(BattleMap.DeltaToPos(i) == tileAction.Origin
                            ? TileSelectorState.OnLayer
                            : TileSelectorState.OffLayer);
                    break;
                default:
                    ResetTilesState();
                    break;
            }
        }

        private void ResetTilesState()
        {
            foreach (var Tile in BattleMap.Tiles)
                Tile.TileSelector.SetState(TileSelectorState.NoCurrentLayer);
        }

        void UnHover()
        {
            
            foreach (var tile in BattleMap.Tiles)
                tile.TileSelector.SetColor(null);

            if (Hovered == -1) return;
            
            var pos = BattleMap.DeltaToPos(Hovered);
            for (int x = 0; x < BattleMap.Size[0]; x++)
                GetTile(BattleMap.PosToDelta(new Vector2Int(x, pos.y))).Refresh();
            for (int y = 0; y < BattleMap.Size[1]; y++)
                GetTile(BattleMap.PosToDelta(new Vector2Int(pos.x, y))).Refresh();
            
            int BlockingViewEntityDelta = BattleMap.PosToDelta(pos + Vector2Int.one);
            if (BlockingViewEntityDelta != -1)
                GetTile(BlockingViewEntityDelta).SetEntityTransparent(false);
        }
        
        void CheckHover(Vector2 mousePos)
        {
            UnHover();
            
            var index = BattleMap.Tiles.FindIndex(x => x.TileSelector.isOverMe(mousePos));
            Hovered = index;
            
            if (index == -1)
            {
                OnHover.Invoke(null);
                return;
            }
            
            var pos = BattleMap.DeltaToPos(index);
            
            OnHover.Invoke(pos);

            var selector = GetTile(index);

            for (int x = 0; x < BattleMap.Size[0]; x++)
                GetTile(BattleMap.PosToDelta(new Vector2Int(x, pos.y))).HalfHover();
            for (int y = 0; y < BattleMap.Size[1]; y++)
                GetTile(BattleMap.PosToDelta(new Vector2Int(pos.x, y))).HalfHover();
            selector.Hover();

            int BlockingViewEntityDelta = BattleMap.PosToDelta(pos + Vector2Int.one);
            if (BlockingViewEntityDelta != -1)
                GetTile(BlockingViewEntityDelta).SetEntityTransparent(true);
        }

        private void Update()
        {
            if(!SelectionCamera) return;
            var mousePos = SelectionCamera.ScreenToWorldPoint(Input.mousePosition);
            
            if (!HoveredTile?.isOverMe(mousePos) ?? true)
                CheckHover(mousePos);

            if (Input.GetMouseButtonDown(0))
                Select(mousePos);

            if (Input.GetMouseButtonDown(1))
                Cancel(mousePos);
        }

        void Select(Vector2 MousePos)
        {
            if (Hovered == -1) return;

            var pos = BattleMap.DeltaToPos(Hovered);
            OnSelect.Invoke(pos);

            //Put hovered tile back in hover mode
            CheckHover(MousePos);
        }

        void Cancel(Vector2 MousePos)
        {
            OnCancel.Invoke();

            //Put hovered tile back in hover mode
            CheckHover(MousePos);
        }
    }
}
