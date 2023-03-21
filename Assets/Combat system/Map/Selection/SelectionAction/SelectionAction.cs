using System;
using System.Collections.Generic;
using CombatSystem.Abilities;
using CombatSystem.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Selection
{
    public class SelectionAction : SelectionLayer
    {
        public static readonly UnityEvent<SelectionAction> OnActionSelection = new();

        public readonly List<Ability> Abilities = new();

        public readonly CombatEntity Caster;
        public readonly Vector2Int Origin;
        
        private readonly Action<SelectionAction, int> OnSelected;
        private readonly Action<SelectionAction> OnCancel;
        private readonly Action<SelectionAction, int?> OnHover;

        public SelectionAction(IEnumerable<Ability> Abilities,
            CombatEntity Caster,
            Vector2Int Origin,
            Action<SelectionAction, int> OnSelected,
            Action<SelectionAction> OnCancel,
            Action<SelectionAction, int?> OnHover)
        {
            this.Abilities.AddRange(Abilities);
            this.Caster = Caster;
            this.Origin = Origin;
            
            this.OnSelected = OnSelected;
            this.OnCancel = OnCancel;
            this.OnHover = OnHover;
            
            OnActionSelection?.Invoke(this);
        }   
        
        public void Select(int index) => OnSelected?.Invoke(this, index);
        public void Cancel() => OnCancel?.Invoke(this);

        public void Hover(int? index) => OnHover?.Invoke(this, index);
    }
}
