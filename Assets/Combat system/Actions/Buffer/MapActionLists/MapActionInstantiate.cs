using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Actions
{
    public class MapActionInstantiate : MapAction
    {
        private CombatEntity Prefab;
        private Vector2Int Position;

        private CombatEntity entity;
        
        public MapActionInstantiate(CombatEntity Prefab, Vector2Int Position)
        {
            this.Prefab = Prefab;
            this.Position = Position;
        }
        
        public override void Apply()
        {
            entity = Object.Instantiate(Prefab, MapView.EntitiesParent);
            BattleMap.MoveEntity.Invoke(entity, Position);
        }

        public override void Cancel()
        {
            BattleMap.RemoveEntity(entity);
            Object.Destroy(entity.gameObject);
            entity = null;
        }

        public void CreateView()
        {
            var view = new MapActionInstantiateView(entity);
            MapActionViewBuffer.AddActionView.Invoke(view);
        }
    }
}