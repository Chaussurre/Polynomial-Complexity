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

        public Dictionary<CombatEntity, Vector2Int> CombatEntities = new();

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
            if (CombatEntities.ContainsValue(position))
                return;
            
            GetEntityTile(entity)?.RemoveCombatEntity();
            CombatEntities[entity] = position;
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
