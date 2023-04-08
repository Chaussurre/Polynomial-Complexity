using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using UnityEngine;

namespace CombatSystem
{
    public class TurnAgent : MonoBehaviour
    {
        public List<ActionManager> ActionManagers;

        private void Start()
        {
            TurnManager.AddAgent.Invoke(this);
        }

        private void OnDestroy()
        {
            TurnManager.RemoveAgent.Invoke(this);
        }

        public void NextTurnStep()
        {
            TurnManager.NextTurnStep?.Invoke();
        }

        public bool ResetActionManager(string ManagerID)
        {
            var manager = ActionManagers.FirstOrDefault(x => x.ID == ManagerID); 
            manager?.ResetTurn();

            return manager;
        }
        
        public bool DoAction(string ManagerID)
        {
            var manager = ActionManagers.FirstOrDefault(x => x.ID == ManagerID);

            return manager && manager.SelectAction();
        }

        public void EndTurn()
        {
            foreach (var actionManager in ActionManagers) 
                actionManager.EndTurn();
        }
    }
}
