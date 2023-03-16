using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class CombatEntityView : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> SpriteRenderers;

        public virtual void SetOrdering(int order)
        {
            foreach (var spriteRenderer in SpriteRenderers) spriteRenderer.sortingOrder = order--;
        }

        public virtual void SetAlpha(float alpha)
        {
            foreach (var renderer in SpriteRenderers)
            {
                var color = renderer.color;
                renderer.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
    }
}
