using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Actions
{
    
    public class MapActionViewMovement : MapActionView
    {
        private readonly float Timer;
        private float currentTime;
        private readonly CombatEntityView entityView;

        private readonly TileView from;
        private readonly TileView to;

        private readonly string ActionAnimationName;
        
        public MapActionViewMovement(float timer, string animationBoolName,
            CombatEntity combatEntity, Vector2Int origin, Vector2Int position)
        {
            ActionAnimationName = animationBoolName;
            Timer = timer;
            entityView = combatEntity.View;
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
            entityView.SetPosition(Vector3.Lerp(posFrom, posTo, currentTime / Timer));
        }

        public override void Start()
        {
            currentTime = 0;
            //deactivating view updates
            entityView.enabled = false;
            
            from.SetCombatEntityAsAbove(entityView);
            to.SetCombatEntityAsAbove(entityView);

            SetAnimatorParameter(entityView, ActionAnimationName, true, AnimatorControllerParameterType.Bool);
        }

        public override void End()
        {
            //reactivating view updates
            entityView.enabled = true;
            
            SetAnimatorParameter(entityView, ActionAnimationName, false, AnimatorControllerParameterType.Bool);
        }
    }
}