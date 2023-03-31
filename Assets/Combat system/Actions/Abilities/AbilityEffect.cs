using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Abilities
{
    public abstract class AbilityEffect<TAbility> : ScriptableObject where TAbility : Ability
    {
        public abstract void Cast(TAbility ability);
    }
}
