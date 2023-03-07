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

        [SerializeField] private Color OffLayerColor = Color.white;
        [SerializeField] private float OffLayerSize;
        
        [SerializeField] private Color HoveredColor = Color.white;
        [SerializeField] private float HoveredSize;
        
        public Collider2D Collider;
        public BattleMap BattleMap { get; set; }

        private TileSelectorState TileSelectorState = TileSelectorState.NoCurrentLayer;
        
        private void Start()
        {
            BattleMap = GetComponentInParent<BattleMap>();
        }

        public bool isOverMe(Vector2 pos)
        {
            return Collider.OverlapPoint(pos);
        }

        public void SetState(TileSelectorState state)
        {
            TileSelectorState = state;
            UnHover();
        }

        public void Hover()
        {
            if (TileSelectorState == TileSelectorState.OffLayer)
                SetSizeColor(0, HoveredColor);
            else
                //lerping when targeted
                SetSizeColor(HoveredSize, HoveredColor, TileSelectorState == TileSelectorState.OnLayer); 
        }

        public void UnHover()
        {
            //lerping when targeted
            SetSizeColor(0, Color.white, TileSelectorState == TileSelectorState.OnLayer);
        }
        
        void SetSizeColor(float Size, Color color, bool LerpColor = false)
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
            
            if (LerpColor)
                TileView.LerpSpriteColor(color * defaultColor);
            else
                TileView.InfluenceSpriteColor(color * defaultColor);
        }
    }
}
