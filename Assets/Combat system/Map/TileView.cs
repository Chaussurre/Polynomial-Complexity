using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Entities;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace CombatSystem.Map
{
    public class TileView : MonoBehaviour
    {
        public SpriteRenderer TopRenderer;
        public SpriteRenderer BorderRenderer;

        [HideInInspector] public CombatEntity CombatEntity;
        private int CombatEntityOrder;
        
        [SerializeField] private float TweenDuration;
        [SerializeField] private float TweenDelay;
        [SerializeField] private Ease TweenEase;
        
        
        private float DefaultSize;
        private Color DefaultColor = Color.white;
        [Space]
        [SerializeField] private float EntityAlphaLower = 0.5f;
        private bool entityIsTransparent;

        private float CurrentSize;

        void Awake()
        {
            DefaultColor = TopRenderer.color;
            DefaultSize = BorderRenderer.size.y;
            ResetSizeAndColor();
        }

        public void Initialize(int sortingOrder)
        {
            TopRenderer.sortingOrder = sortingOrder * 10;
            BorderRenderer.sortingOrder = sortingOrder * 10;
            CombatEntityOrder = sortingOrder * 10 + 9;
        }

        public void ResetSizeAndColor()
        {
            SetSpriteSize(DefaultSize);
            SetSpriteColor(DefaultColor);
        }

        public void SetSpriteSize(float size)
        {
            if (Math.Abs(size - CurrentSize) < 0.01f)
                return;
            
            CurrentSize = size;
            
            TopRenderer.transform.DOKill();
            //var newTween = TopRenderer.transform
            TopRenderer.transform
                .DOLocalMove(Vector3.up * size, TweenDuration)
                .SetDelay(TweenDelay)
                .SetEase(TweenEase)
                .OnUpdate(() =>
                {
                    BorderRenderer.size = new Vector2(1, TopRenderer.transform.localPosition.y);
                });
        }

        public void SetSpriteColor(Color color)
        {
            TopRenderer.color = color;
        }

        public void InfluenceSpriteSize(float size)
        {
            SetSpriteSize(DefaultSize + size);
        }

        public void InfluenceSpriteColor(Color color)
        {
            SetSpriteColor(DefaultColor * color);
        }

        public void LerpSpriteColor(Color color)
        {
            SetSpriteColor(Color.Lerp(DefaultColor , color, 0.5f));
        }

        public void RefreshCombatEntity()
        {
            if (CombatEntity)
            {
                CombatEntity.View.SetPosition(GetCombatEntityEmplacement());
                CombatEntity.View.SetOrdering(CombatEntityOrder);
                
                CombatEntity.View.SetAlpha(entityIsTransparent ? EntityAlphaLower : 1f);
            }
        }

        public Vector3 GetCombatEntityEmplacement()
        {
            return TopRenderer.transform.position;
        }

        public void SetCombatEntityAsAbove(CombatEntityView combatEntityView)
        {
            combatEntityView.SetOrderingMinimum(CombatEntityOrder);
        }

        public void SetTransparent(bool value)
        {
            entityIsTransparent = value;
        }
    }
}
