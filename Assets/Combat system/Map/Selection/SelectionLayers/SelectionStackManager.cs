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
        /// The argument is the new top SelectionLayer.
        /// </summary>
        public static UnityEvent<SelectionLayer> OnLayerBecomeActive = new();
        
        /// <summary>
        /// Called whenever the top Selection is canceled.
        /// The argument is the new top SelectionLayer.
        /// </summary>
        public static UnityEvent<SelectionLayer> OnCancel = new();
        
        #endregion

        [SerializeField] private bool CanCancelLastChoice;
        
        private readonly Stack<SelectionLayer> SelectionLayers = new();

        private SelectionLayer LastLayer => SelectionLayers.First();

        private void Awake()
        {
            AddLayer.AddListener(AddSelectionLayer);
            ClearStack.AddListener(OnClearStack);
            
            MapSelectionManager.OnSelect.AddListener(OnSelect);
            MapSelectionManager.OnCancel.AddListener(CancelLayer);
            MapSelectionManager.OnHover.AddListener(OnHover);
        }

        private void AddSelectionLayer(SelectionLayer Layer)
        {
            SelectionLayers.Push(Layer);
            
            OnLayerBecomeActive.Invoke(Layer);
        }
        
        private void OnSelect(Vector2Int pos)
        {
            if (SelectionLayers.Count <= 0) return;
            
            if (LastLayer is SelectionTile TileLayer && TileLayer.Positions.Contains(pos))
                TileLayer.Select(pos);
        }

        private void CancelLayer()
        {
            if (SelectionLayers.Count == 1 && !CanCancelLastChoice)
                return;
            
            if(SelectionLayers.TryPop(out var layer))
                layer.Cancel();

            SelectionLayers.TryPeek(out var newTop);
            
            OnLayerBecomeActive.Invoke(newTop);
            OnCancel.Invoke(newTop);
        }

        private void OnHover(Vector2Int? pos)
        {
            if (SelectionLayers.Count > 0 && LastLayer is SelectionTile tileLayer)
                    tileLayer.Hover(pos);
        }

        private void OnClearStack()
        {
            SelectionLayers.Clear();
        }
    }
}
