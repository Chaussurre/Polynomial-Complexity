using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CombatSystem.Map
{
    public class MapView : MonoBehaviour
    {
        private Vector3 LeftUpCorner; //The corner from which we draw tiles
        
        [SerializeField] private Vector2 TileSize;
        [SerializeField] private float Slope;

        public void Start()
        {
            var Size = BattleMap.Size;
            var Tiles = BattleMap.Tiles;

            //Set LeftUpCorner
            var y = (Size[1] - 1) / 2.0f;
            var x = -(Size[0] - 1) / 2.0f;
            LeftUpCorner = new Vector3(x * TileSize.x + Slope * y, y * TileSize.y);

            for (int i = 0; i < Tiles.Count; i++)
            {
                var tile = Tiles[i];
                var pos = BattleMap.DeltaToPos(i);
                tile.Initialize(
                    new Vector3(pos.x * TileSize.x + Slope * -pos.y, -pos.y * TileSize.y) + LeftUpCorner,
                    i);
            }
        }
    }
}
