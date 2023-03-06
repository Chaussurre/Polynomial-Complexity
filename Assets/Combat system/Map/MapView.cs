using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CombatSystem.Map
{
    public class MapView : MonoBehaviour
    {
        private Map map;
        private Vector3 LeftDownCorner; //The corner from which we draw tiles
        
        [SerializeField] private Vector2 TileSize;
        [SerializeField] private float Slope;

        public void Initialize(Map map)
        {
            this.map = map;
            
            //Set LeftDownCorner
            {
                var y = -(map.Size[1] - 1) / 2.0f;
                var x = -(map.Size[0] - 1) / 2.0f;
                LeftDownCorner = new Vector3(x * TileSize.x  + Slope * y, y * TileSize.y);
            }
            
            for (int i = 0; i < map.Tiles.Count; i++)
            {
                var tile = map.Tiles[i];
                var pos = map.DeltaToPos(i);
                tile.SetPos(new Vector3(pos.x * TileSize.x + Slope * pos.y, pos.y * TileSize.y) + LeftDownCorner);
            }
        }
    }
}
