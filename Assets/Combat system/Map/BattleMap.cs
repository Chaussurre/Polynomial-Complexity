using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Map
{
    public class BattleMap : MonoBehaviour
    {
        private MapView View;
        
        public Vector2Int Size;
        public readonly List<Tile> Tiles = new();
        public Tile TilePrefab;
        
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
