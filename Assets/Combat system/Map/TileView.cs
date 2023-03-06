using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Map
{
    public class TileView : MonoBehaviour
    {
        public SpriteRenderer TopRenderer;
        public SpriteRenderer BorderRenderer;

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
            TopRenderer.transform.localPosition = Vector3.up * (size - 1);
            BorderRenderer.size = new Vector2(1, size);
        }

        void SetSpriteColor(Color color)
        {
            TopRenderer.color = color;
        }
    }
}
