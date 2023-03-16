using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using CombatSystem.Map;

namespace CombatSystem.Selection
{
    public enum TileSelectorState
    {
        NoCurrentLayer,
        OnLayer,
        OffLayer,
    }
    
    public class TileSelector : MonoBehaviour
    {
        [SerializeField] private TileView TileView;
        [Space]
        [SerializeField] private Color OnLayerColor = Color.white;
        [SerializeField] private float OnLayerSize;
        [Space]
        [SerializeField] private Color OffLayerColor = Color.white;
        [SerializeField] private float OffLayerSize;
        [Space]
        [SerializeField] private Color HoveredColor = Color.white;
        [SerializeField] private float HoveredSize;
        [Space]
        [SerializeField] private Color HalfHoveredColor = Color.white;
        [SerializeField] private float HalfHoveredSize;
        
        private Color? ForceColor = null;
        
        public Collider2D Collider;

        private TileSelectorState TileSelectorState = TileSelectorState.NoCurrentLayer;

        public bool isOverMe(Vector2 pos)
        {
            return Collider.OverlapPoint(pos);
        }

        public void SetState(TileSelectorState state)
        {
            TileSelectorState = state;
            Refresh();
        }

        public void SetColor(Color? color)
        {
            if (color != ForceColor)
            {
                ForceColor = color;
                if (color.HasValue)
                    TileView.SetSpriteColor(color.Value);
                else
                    Refresh();
            }
        }
        
        public void Hover()
        {
            if (TileSelectorState == TileSelectorState.OffLayer)
                SetSizeColor(0, HoveredColor);
            else
                SetSizeColor(HoveredSize, HoveredColor); 
        }

        public void HalfHover()
        {
            if (TileSelectorState == TileSelectorState.OffLayer)
                SetSizeColor(0, HalfHoveredColor);
            else
                SetSizeColor(HalfHoveredSize, HalfHoveredColor);
        }

        public void Refresh()
        {
            SetSizeColor(0, Color.white);
        }

        private void SetSizeColor(float Size, Color color)
        {
            float defaultSize;
            Color defaultColor;
            switch (TileSelectorState)
            {
                case TileSelectorState.OffLayer:
                    defaultSize = OffLayerSize;
                    defaultColor = OffLayerColor;
                    break;
                case TileSelectorState.OnLayer:
                    defaultSize = OnLayerSize;
                    defaultColor = OnLayerColor;
                    break;
                default:
                    defaultSize = 0;
                    defaultColor = Color.white;
                    break;
            }
            
            TileView.InfluenceSpriteSize(defaultSize + Size);
            
            if (ForceColor.HasValue)
                TileView.SetSpriteColor(ForceColor.Value);
            
            else if (TileSelectorState == TileSelectorState.OnLayer)
                TileView.LerpSpriteColor(color * defaultColor);
            else
                TileView.InfluenceSpriteColor(color * defaultColor);
        }

        public void SetEntityTransparent(bool value) => TileView.SetTransparent(value);
    }
}
