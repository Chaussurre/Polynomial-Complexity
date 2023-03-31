using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool ResetActionManager(string ManagerID)
        {
            var manager = ActionManagers.FirstOrDefault(x => x.ID == ManagerID); 
            manager?.ResetTurn();

            return manager;
        }
        
        public bool DoAction(Vector2Int position, string ManagerID)
        {
            var manager = ActionManagers.FirstOrDefault(x => x.ID == ManagerID);

            return manager && manager.SelectAction(position);
        }

        public void EndTurn()
        {
            foreach (var actionManager in ActionManagers) 
                actionManager.EndTurn();
        }
    }
}
