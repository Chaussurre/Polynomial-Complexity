using System;
using System.Collections.Generic;
using CombatSystem.Entities;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Abilities
{
    public class AbilityManager : ActionManager
    {
        private CombatEntity Entity;
        private TurnAgent Owner;

        public List<Ability> Abilities;

        private SelectionAction SelectionAction;

        public override string ID => "Abilities";

        private void Awake()
        {
            Entity = GetComponentInParent<CombatEntity>();
            Owner = GetComponentInParent<TurnAgent>();
            
            SelectionAction = new SelectionAction(
                Abilities,
                Entity,
                Vector2Int.zero,
                OnSelect,
                null,//todo
                null);//todo
        }

        public override bool SelectAction()
        {
            if (Abilities.Count == 0)
                return false;

            SelectionAction.Origin = BattleMap.GetEntityPos(Entity);

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
