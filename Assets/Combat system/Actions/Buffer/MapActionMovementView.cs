using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Actions
{
    
    public class MapActionViewMovement : MapActionView
    {
        private float Timer;
        private float currentTime;
        private CombatEntity Entity;

        private TileView from;
        private TileView to;
        
        public MapActionViewMovement(float timer, CombatEntity combatEntity, Vector2Int origin, Vector2Int position)
        {
            Timer = timer;
            Entity = combatEntity;
            from = BattleMap.Tiles[BattleMap.PosToDelta(origin)].TileView;
            to = BattleMap.Tiles[BattleMap.PosToDelta(position)].TileView;
        }

        public override bool IsOver => currentTime >= Timer; 
        public override void Update(float deltaTime)
        {
            if (Timer == 0)
                return;
            
            currentTime += deltaTime;
            var posFrom = from.GetCombatEntityEmplacement();
            var posTo = to.GetCombatEntityEmplacement();
            Entity.View.SetPosition(Vector3.Lerp(posFrom, posTo, currentTime / Timer));
        }

        public override void Start()
        {
            currentTime = 0;
            //deactivating view updates
            Entity.View.enabled = false;
            
            from.SetCombatEntityAsAbove(Entity);
            to.SetCombatEntityAsAbove(Entity);
        }

        public override void End()
        {
            //reactivating view updates
            Entity.View.enabled = true;
        }
    }
}