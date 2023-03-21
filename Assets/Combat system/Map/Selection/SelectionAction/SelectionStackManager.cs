using System;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Map;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace CombatSystem.Selection
{
    public class SelectionStackManager : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Require to the player to select a game element
        /// </summary>
        public static UnityEvent<SelectionLayer> AddLayer = new();
        
        /// <summary>
        /// Clear the stack of selections.
        /// </summary>
        public static UnityEvent ClearStack = new();

        /// <summary>
        /// Called whenever the top Selection Layer changes.
        /// The argument is the new SelectionLayer.
        /// </summary>
        public static UnityEvent<SelectionLayer> OnLayerBecomeActive = new();
        
        #endregion
        
        private readonly Stack<SelectionLayer> SelectionLayers = new();

        private SelectionLayer LastLayer => SelectionLayers.First();

        private void Awake()
        {
            AddLayer.AddListener(AddSelectionLayer);
            ClearStack.AddListener(OnClearStack);
            
            MapSelectionManager.OnSelect.AddListener(OnSelect);
            MapSelectionManager.OnCancel.AddListener(OnCancel);
            MapSelectionManager.OnHover.AddListener(OnHover);
        }

        private void AddSelectionLayer(SelectionLayer Layer)
        {
            SelectionLayers.Push(Layer);
            
            OnLayerBecomeActive?.Invoke(Layer);
            RefreshSelection();
        }
        
        private void OnSelect(Vector2Int pos)
        {
            if (SelectionLayers.Count > 0)
            {
                if (LastLayer is SelectionTile TileLayer && TileLayer.Positions.Contains(pos))
                    TileLayer.Select(pos);
            }
            else
                MapSelectionManager.TrySelectEntity?.Invoke(pos);
        }

        private void OnCancel()
        {
            if(SelectionLayers.TryPop(out var layer))
                layer.Cancel();
            
            if(SelectionLayers.TryPeek(out var newTop))
                OnLayerBecomeActive?.Invoke(newTop);
            
            RefreshSelection();
        }

        private void OnHover(Vector2Int? pos)
        {
            if (SelectionLayers.Count > 0 && LastLayer is SelectionTile tileLayer)
                    tileLayer.Hover(pos);
        }

        private void OnClearStack()
        {
            SelectionLayers.Clear();
            
            RefreshSelection();
        }

        private void RefreshSelection()
        {
            MapSelectionManager.RefreshSelection?
                .Invoke(SelectionLayers.Count > 0 && LastLayer is SelectionTile selectionTile
                    ? selectionTile.Positions
                    : null);
        }
    }
}
