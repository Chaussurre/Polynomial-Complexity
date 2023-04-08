using CombatSystem.Entities;
using UnityEngine;

namespace CombatSystem.Abilities
{
    
    public abstract class Ability : MonoBehaviour
    {
        public string AbilityName = "Ability";
        public Sprite Icon;
            
        [HideInInspector] public CombatEntity Caster;
        
        public virtual void Start()
        {
            Caster = GetComponentInParent<CombatEntity>();
        }

        public abstract void Select(Vector2Int Position);
        
        public virtual void FinishedSelect(Vector2Int position)
        {
            Cast();
            Caster.NextTurnStep(position);
        }

        protected abstract void Cast();
    }
}