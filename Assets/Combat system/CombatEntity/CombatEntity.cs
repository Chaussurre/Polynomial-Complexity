using System;
using System.Collections.Generic;
using CombatSystem.Abilities;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Entities
{
    public class CombatEntity : MonoBehaviour
    {
        public bool CanTakeTurn = true;
        
        public CombatEntityView View; 
        
        public HealthStatus HealthStatus;

        public List<ActionManager> ActionManagers;

        public void NextTurnStep(Vector2Int position)
        {
            TurnManager.NextTurnStep?.Invoke(position);
        }

        public void ResetActionManager(int ManagerIndex)
        {
            if (ManagerIndex < ActionManagers.Count)
                ActionManagers[ManagerIndex].ResetTurn();
        }
        
        public void DoAction(Vector2Int position, int ManagerIndex)
        {
            if (ManagerIndex >= ActionManagers.Count)
                return;
            
            if (!ActionManagers[ManagerIndex].SelectAction(position))
                NextTurnStep(position);
        }

        public void EndTurn()
        {
            foreach (var actionManager in ActionManagers) 
                actionManager.EndTurn();
        }
    }
}
