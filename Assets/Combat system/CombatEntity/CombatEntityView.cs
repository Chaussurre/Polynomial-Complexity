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
    }
}
