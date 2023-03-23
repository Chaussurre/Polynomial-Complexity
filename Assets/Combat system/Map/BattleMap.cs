using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Dictionary<CombatEntity, Vector2Int> CombatEntities = new();

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
            GetEntityTile(entity)?.RemoveCombatEntity();
            CombatEntities[entity] = position;
            GetEntityTile(entity).AddCombatEntity(entity);
        }

        public static int CountEntity(Vector2Int position)
        {
            return ActiveMap.CombatEntities.Count(x => x.Value == position);
        }

        public static IEnumerable<CombatEntity> GetEntities(Vector2Int position)
        {
            return ActiveMap.CombatEntities
                .Where(x => x.Value == position)
                .Select(x => x.Key);
        }
        
        public static Tile GetEntityTile(CombatEntity entity)
        {
            if (!ActiveMap) return null;
            
            if (!ActiveMap.CombatEntities.ContainsKey(entity))
                return null;
            
            return Tiles[PosToDelta(ActiveMap.CombatEntities[entity])];
        }

        public static Vector2Int GetEntityPos(CombatEntity entity)
        {
            return ActiveMap.CombatEntities[entity];
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
