using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class MovementManager : MonoBehaviour
    {
        private CombatEntity Entity;
        public SelectionLayerFilter MovementFilter;

        public int Speed;
        public int RemainingMoves { get; set; }

        private void Start()
        {
            Entity = GetComponent<CombatEntity>();
            
        }

        public void Move(Vector2Int position, MapSelectionManager MapSelection)
        {
            MapSelection.BattleMap.MoveCombatEntity(Entity, position);
            MapSelection.EndStack();
        }
    }
}
