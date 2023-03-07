using System.Collections;
using System.Collections.Generic;
using CombatSystem.Entities;
using DG.Tweening;
using UnityEngine;

namespace CombatSystem.Map
{
    public class TileView : MonoBehaviour
    {
        public SpriteRenderer TopRenderer;
        public SpriteRenderer BorderRenderer;

        [HideInInspector] public CombatEntity CombatEntity;
        
        [SerializeField] private float TweenDuration;
        [SerializeField] private Ease TweenEase;
        
        private float DefaultSize;
        private Color DefaultColor = Color.white;
        void Start()
        {
            DefaultColor = TopRenderer.color;
            DefaultSize = BorderRenderer.size.y;
            ResetSizeAndColor();
        }

        public void Initialize(int sortingOrder)
        {
            TopRenderer.sortingOrder = sortingOrder;
            BorderRenderer.sortingOrder = sortingOrder;
        }

        public void ResetSizeAndColor()
        {
            SetSpriteSize(DefaultSize);
            SetSpriteColor(DefaultColor);
        }

        public void SetSpriteSize(float size)
        {
            TopRenderer.transform.DOKill();
            TopRenderer.transform.DOLocalMove(Vector3.up * size, TweenDuration)
                .SetEase(TweenEase)
                .OnUpdate(() =>
            {
                BorderRenderer.size = new Vector2(1, TopRenderer.transform.localPosition.y);
                
                RefreshCombatEntity();
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
                CombatEntity.transform.position = TopRenderer.transform.position;
                CombatEntity.SpriteRenderer.sortingOrder = TopRenderer.sortingOrder;
            }
        }
    }
}
