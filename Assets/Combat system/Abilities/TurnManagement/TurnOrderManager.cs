using System;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class TurnOrderManager : MonoBehaviour
    {
        [SerializeField] private List<CombatEntity> Entities = new();

        private int turnIndex = -1;

        private void Awake()
        {
            TurnManager.EndTurn.AddListener(NextTurn);
        }

        private void Start()
        {
            NextTurn();
        }

        private void NextTurn()
        {
            turnIndex++;
            turnIndex %= Entities.Count;
            TurnManager.StartEntityTurn.Invoke(Entities[turnIndex]);
        }
    }
}