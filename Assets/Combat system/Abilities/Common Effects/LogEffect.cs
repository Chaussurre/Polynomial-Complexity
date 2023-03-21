using CombatSystem.Entities;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Abilities
{
    [CreateAssetMenu(fileName = "Log Effect", menuName = "Combat System/Abilities/Effects/Log Effect")]
    public class LogEffect : TargetPositionEffect
    {
        public string message;

        protected override void ApplyEffect(Vector2Int position)
        {
            Debug.Log($"message : {message} ; position : {position}");
        }
    }
}
