using System;
using System.Collections.Generic;
using CombatSystem.Entities;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Abilities
{
    public class AbilityManager : ActionManager
    {
        private CombatEntity Entity;

        public List<Ability> Abilities;

        private SelectionAction SelectionAction;
        
        private void Start()
        {
            Entity = GetComponentInParent<CombatEntity>();
            SelectionAction = new SelectionAction(
                Abilities,
                Entity,
                Vector2Int.zero,
                OnSelect,
                null,//todo
                null);//todo
        }

        public override bool SelectAction(Vector2Int Origin)
        {
            if (Abilities.Count == 0)
                return false;

            SelectionAction.Origin = Origin;

            SelectionStackManager.AddLayer?.Invoke(SelectionAction);

            return true;
        }

        private void OnSelect(SelectionAction ActionLayer, int selection)
        {
            Abilities[selection].Select(ActionLayer.Origin);
        }

        public override void ResetTurn() { }

        public override void EndTurn() { }
    }
}
