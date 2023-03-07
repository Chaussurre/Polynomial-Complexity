using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Entities;
using UnityEngine;

namespace CombatSystem.Map
{
    public class BattleMap : MonoBehaviour
    {
        [Serializable]
        public struct InitCombatEntity
        {
            public CombatEntity Entity;
            public Vector2Int Position;
        }

        public List<InitCombatEntity> InitializeCombatEntities = new();
        
        private MapView View;
        
        public Vector2Int Size;
        public readonly List<Tile> Tiles = new();
        public Tile TilePrefab;

        public readonly Dictionary<CombatEntity, Vector2Int> CombatEntities = new();
        public readonly Dictionary<Vector2Int, CombatEntity> CombatEntitiesPos = new();

        private void Awake()
        {
            View = GetComponent<MapView>();
            
            if (!TilePrefab)
            {
                Debug.LogError("No tile prefab for map");
                return;
            }
            
            
            for(int x = 0; x < Size[0]; x++)
            for (int y = 0; y < Size[1]; y++)
            {
                var tile = Instantiate(TilePrefab, transform);
                Tiles.Add(tile);
            }
            
            View?.Initialize(this);

            foreach (var entity in InitializeCombatEntities) 
                MoveCombatEntity(entity.Entity, entity.Position);
        }

        public void MoveCombatEntity(CombatEntity entity, Vector2Int position)
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

        public Tile GetEntityTile(CombatEntity entity)
        {
            if (!CombatEntities.ContainsKey(entity))
                return null;
            
            return Tiles[PosToDelta(CombatEntities[entity])];
        }

        public int PosToDelta(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Size[0] || pos.y < 0 || pos.y >= Size[1])
                return -1;
            
            return pos.y * Size[0] + pos.x;
        }

        public Vector2Int DeltaToPos(int delta)
        {
            var x = delta % Size[0];
            var y = delta / Size[0];

            return new Vector2Int(x, y);
        }
    }
}
