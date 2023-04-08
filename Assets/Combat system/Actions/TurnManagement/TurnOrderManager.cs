using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class TurnOrderManager : MonoBehaviour
    {
        [SerializeField] private List<TurnAgent> Agents = new();

        private int turnIndex = -1;

        private void Awake()
        {
            TurnManager.OnEndTurn.AddListener(NextTurn);
            
            TurnManager.AddAgent.AddListener(AddAgent);
            TurnManager.RemoveAgent.AddListener(RemoveAgent);
        }

        private void AddAgent(TurnAgent Agent)
        {
            if (!Agents.Contains(Agent))
                Agents.Add(Agent);
        }

        private void RemoveAgent(TurnAgent Agent)
        {
            Agents.Remove(Agent);
        }

        private void Start()
        {
            NextTurn();
        }

        private void NextTurn()
        {
            turnIndex++;
            turnIndex %= Agents.Count;
            TurnManager.StartAgentTurn.Invoke(Agents[turnIndex]);
        }
    }
}