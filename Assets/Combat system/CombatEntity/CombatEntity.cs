using CombatSystem.Abilities;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class CombatEntity : MonoBehaviour
    {
        public CombatEntityView View;
        
        public HealthStatus HealthStatus;

        public AbilityManager AbilityManager;

        public MovementManager MovementManager;
        
        public bool Select(Vector2Int position)
        {
            if (MovementManager)
            {
                MovementManager.SelectMove(position);
                return true;
            }

            if (AbilityManager)
            {
                AbilityManager.SelectAbilities(position);
                return true;
            }

            return false;
        }

        public void FinishMovement(Vector2Int position)
        {
            if (AbilityManager)
                AbilityManager.SelectAbilities(position);
            else
                SelectionStackManager.ClearStack?.Invoke();
        }
    }
}
