using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Entities;
using CombatSystem.Selection;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Map
{
    public class BattleMap : MonoBehaviour
    {
        #region Events

        public static UnityEvent<CombatEntity, Vector2Int> MoveEntity = new();

        #endregion
        
        private static BattleMap ActiveMap = null;
        
        [Serializable]
        public struct InitCombatEntity
        {
            public CombatEntity Entity;
            public Vector2Int Position;
        }

        public List<InitCombatEntity> InitializeCombatEntities = new();
        
        private readonly List<Tile> tiles = new();
        public static List<Tile> Tiles => ActiveMap.tiles;
        
        [SerializeField] private Vector2Int size;
        public static Vector2Int Size => ActiveMap.size;
        
        [SerializeField] private Tile TilePrefab;

        public readonly Dictionary<CombatEntity, Vector2Int> CombatEntities = new();
        public readonly Dictionary<Vector2Int, CombatEntity> CombatEntitiesPos = new();

        private void Awake()
        {
            ActiveMap = this;
            
            MoveEntity.AddListener(MoveCombatEntity);
            
            if (!TilePrefab)
            {
                Debug.LogError("No tile prefab for map");
                return;
            }
            
            
            for(int x = 0; x < size[0]; x++)
            for (int y = 0; y < size[1]; y++)
            {
                var tile = Instantiate(TilePrefab, transform);
                tiles.Add(tile);
            }
            

            foreach (var entity in InitializeCombatEntities) 
                MoveCombatEntity(entity.Entity, entity.Position);
        }

        private void MoveCombatEntity(CombatEntity entity, Vector2Int position)
        {
            if (CombatEntitiesPos.ContainsKey(position))
                return;

            GetEntityTile(entity)?.RemoveCombatEntity();
            
            if (CombatEntities.TryGetValue(entity, out var oldPos))
                CombatEntitiesPos.Remove(oldPos);
            
            CombatEntities[entity] = position;
            CombatEntitiesPos[position] = entity;
            GetEntityTile(entity).AddCombatEntity(entity);
        }

        public static bool HasEntity(Vector2Int position, out CombatEntity entity)
        {
            return ActiveMap.CombatEntitiesPos.TryGetValue(position, out entity);
        }
        
        public static Tile GetEntityTile(CombatEntity entity)
        {
            if (!ActiveMap) return null;
            
            if (!ActiveMap.CombatEntities.ContainsKey(entity))
                return null;
            
            return Tiles[PosToDelta(ActiveMap.CombatEntities[entity])];
        }

        public static int PosToDelta(Vector2Int pos)
        {
            var Size = ActiveMap.size;
            if (pos.x < 0 || pos.x >= Size[0] || pos.y < 0 || pos.y >= Size[1])
                return -1;
            
            return pos.y * Size[0] + pos.x;
        }

        public static Vector2Int DeltaToPos(int delta)
        {
            var Size = ActiveMap.size;
            
            var x = delta % Size[0];
            var y = delta / Size[0];

            return new Vector2Int(x, y);
        }
    }
}
