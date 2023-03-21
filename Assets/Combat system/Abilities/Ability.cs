using System;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Abilities
{
    
    public abstract class Ability : MonoBehaviour
    {
        [HideInInspector] public CombatEntity Caster;
        
        public virtual void Start()
        {
            Caster = GetComponentInParent<CombatEntity>();
        }

        public abstract void Select(Vector2Int Position);


        public abstract void Cast();
    }
}