using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CombatSystem.Map
{
    public class TileView : MonoBehaviour
    {
        public SpriteRenderer TopRenderer;
        public SpriteRenderer BorderRenderer;
        
        [SerializeField] private float TweenDuration;
        [SerializeField] private Ease TweenEase;
        
        [SerializeField] private float DefaultSize;
        [SerializeField] private float SizeWhenHovered;
        [SerializeField] private float SizeWhenSelected;
        
        [SerializeField] private Color DefaultColor = Color.white;
        [SerializeField] private Color ColorWhenHovered = Color.white;
        [SerializeField] private Color ColorWhenSelected = Color.white;
        void Start()
        {
            ResetSizeAndColor();
        }

        public void Initialize(int sortingOrder)
        {
            TopRenderer.sortingOrder = sortingOrder;
            BorderRenderer.sortingOrder = sortingOrder;
        }

        public void Hover()
        {
            SetSpriteSize(SizeWhenHovered);
            SetSpriteColor(ColorWhenHovered);
        }

        public void Select()
        {
            SetSpriteSize(SizeWhenSelected);
            SetSpriteColor(ColorWhenSelected);
        }

        public void ResetSizeAndColor()
        {
            SetSpriteSize(DefaultSize);
            SetSpriteColor(DefaultColor);
        }

        void SetSpriteSize(float size)
        {
            TopRenderer.transform.DOKill();
            TopRenderer.transform.DOLocalMove(Vector3.up * (size - 1), TweenDuration)
                .SetEase(TweenEase)
                .OnUpdate(() =>
            {
                BorderRenderer.size = new Vector2(1, 1 + TopRenderer.transform.localPosition.y);
            });
        }

        void SetSpriteColor(Color color)
        {
            TopRenderer.color = color;
        }
    }
}
