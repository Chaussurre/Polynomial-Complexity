using System;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Map;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Selection
{
    public class SelectionStackManager : MonoBehaviour
    {
        #region Events

        public static UnityEvent<SelectionLayer> AddLayer = new();
        public static UnityEvent ClearStack = new();

        #endregion
        
        private readonly Stack<SelectionLayer> SelectionLayers = new();

        private SelectionLayer LastLayer => SelectionLayers.First();

        private void Awake()
        {
            AddLayer.AddListener(AddTileSelection);
            ClearStack.AddListener(OnClearStack);
            
            MapSelectionManager.OnSelect.AddListener(OnSelect);
            MapSelectionManager.OnCancel.AddListener(OnCancel);
            MapSelectionManager.OnHover.AddListener(OnHover);
        }

        private void AddTileSelection(SelectionLayer Layer)
        {
            SelectionLayers.Push(Layer);
            
            RefreshSelection();
        }
        

        private void OnSelect(Vector2Int pos)
        {
            if (SelectionLayers.Count > 0)
                LastLayer.Select(pos);
            else
                MapSelectionManager.TrySelectEntity?.Invoke(pos);
        }

        private void OnCancel()
        {
            if(SelectionLayers.TryPop(out var layer))
                layer.Cancel();
            
            RefreshSelection();
        }

        private void OnHover(Vector2Int? pos)
        {
            if (SelectionLayers.Count > 0) 
                LastLayer.Hover(pos);
        }

        private void OnClearStack()
        {
            SelectionLayers.Clear();
            
            RefreshSelection();
        }

        private void RefreshSelection()
        {
            MapSelectionManager.RefreshSelection?.Invoke(SelectionLayers.Count > 0 ? LastLayer.Positions : null);
        }
    }
}
