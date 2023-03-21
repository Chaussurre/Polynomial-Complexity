using System;
using System.Collections.Generic;
using CombatSystem.Entities;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Abilities
{
    public class AbilityManager : MonoBehaviour
    {
        private CombatEntity Entity;

        public List<Ability> Abilities;

        private void Start()
        {
            Entity = GetComponent<CombatEntity>();
        }

        public void SelectAbilities(Vector2Int Origin)
        {
            SelectionStackManager.AddLayer?.Invoke(new SelectionAction(
                Abilities,
                Entity,
                Origin,
                OnSelect,
                null, //todo
                null)); //todo
        }

        private void OnSelect(SelectionAction ActionLayer, int selection)
        {
            Abilities[selection].Select(ActionLayer.Origin);
        }
    }
}
