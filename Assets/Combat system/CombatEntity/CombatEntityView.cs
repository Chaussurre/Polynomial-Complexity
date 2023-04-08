using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class CombatEntityView : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> SpriteRenderers;
        private CombatEntity Entity;

        public List<Animator> Animators;

        private void Start()
        {
            Entity = GetComponentInParent<CombatEntity>();
        }

        public virtual void SetOrdering(int order)
        {
            foreach (var spriteRenderer in SpriteRenderers) spriteRenderer.sortingOrder = order--;
        }
        
        public virtual void SetOrderingMinimum(int order)
        {
            if (order < SpriteRenderers[0].sortingOrder)
                return;

            SetOrdering(order);
        }

        public virtual void SetAlpha(float alpha)
        {
            foreach (var renderer in SpriteRenderers)
            {
                var color = renderer.color;
                renderer.color = new Color(color.r, color.g, color.b, alpha);
            }
        }

        public virtual void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void Update()
        {
            var tile = BattleMap.GetEntityTile(Entity);
            tile?.TileView.RefreshCombatEntity();
        }
    }
}
