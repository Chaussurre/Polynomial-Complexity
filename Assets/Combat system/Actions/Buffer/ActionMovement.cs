using System.Collections;
using System.Collections.Generic;
using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem
{
    public class ActionMovement : Action
    {
        private readonly CombatEntity entity;
        private Vector2Int position;
        private Vector2Int origin;
        private bool teleport;
        
        public ActionMovement(CombatEntity entity, Vector2Int origin, Vector2Int  position, bool teleport = false)
        {
            this.entity = entity;
            this.origin = origin;
            this.position = position;
            this.teleport = teleport;
        }
        
        public override void Preview()
        {
            BattleMap.MoveEntity.Invoke(entity, position);
        }

        public override void Apply()
        {
            BattleMap.MoveEntity?.Invoke(entity, position);
        }

        public override void Cancel()
        {
            BattleMap.MoveEntity?.Invoke(entity, origin);
        }
    }
}
