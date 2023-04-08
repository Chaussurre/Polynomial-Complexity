using CombatSystem.Entities;
using UnityEngine;

namespace CombatSystem.Abilities
{
    
    public abstract class Ability : MonoBehaviour
    {
        public string AbilityName = "Ability";
        public Sprite Icon;
            
        [HideInInspector] public TurnAgent Owner;
        
        public virtual void Start()
        {
            Owner = GetComponentInParent<TurnAgent>();
        }

        public abstract void Select(Vector2Int Position);
        
        public virtual void FinishedSelect()
        {
            Cast();
            Owner.NextTurnStep();
        }

        protected abstract void Cast();
    }
}