using System.Collections;
using System.Collections.Generic;
using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Actions
{
    public class MapActionMovement : MapAction
    {
        private readonly CombatEntity entity;
        private Vector2Int position;
        private Vector2Int origin;

        public MapActionMovement(CombatEntity entity, Vector2Int origin, Vector2Int  position)
        {
            this.entity = entity;
            this.origin = origin;
            this.position = position;
        }

        public override void Apply()
        {
            BattleMap.MoveEntity.Invoke(entity, position);
        }

        public override void Cancel()
        {
            BattleMap.MoveEntity?.Invoke(entity, origin);
        }

        public void CreateView(float Timer, string ActionBoolName)
        {
            MapActionViewBuffer.AddActionView.Invoke(new MapActionViewMovement(Timer,
                ActionBoolName,
                entity,
                origin,
                position));
        }

        
    }
}
