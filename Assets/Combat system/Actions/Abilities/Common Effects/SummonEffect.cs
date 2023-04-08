using CombatSystem.Actions;
using CombatSystem.Entities;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Abilities
{
    
    [CreateAssetMenu(fileName = "Summon Effect", menuName = "Combat System/Abilities/Effects/Summon Effect")]
    public class SummonEffect : TargetPositionEffect
    {
        [SerializeField] private CombatEntity Prefab;
        
        protected override void ApplyEffect(Vector2Int position)
        {
            var action = new MapActionInstantiate(Prefab, position);
            MapActionBuffer.AddAction.Invoke(action);
            action.CreateView();
        }

        public override void Cancel()
        {
            MapActionBuffer.PopActions.Invoke(1);
        }
    }
}