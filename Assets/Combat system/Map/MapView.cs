using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CombatSystem.Map
{
    public class MapView : MonoBehaviour
    {
        private BattleMap BattleMap;
        private Vector3 LeftUpCorner; //The corner from which we draw tiles
        
        [SerializeField] private Vector2 TileSize;
        [SerializeField] private float Slope;

        public void Initialize(BattleMap battleMap)
        {
            BattleMap = battleMap;
            
            //Set LeftUpCorner
            {
                var y = (battleMap.Size[1] - 1) / 2.0f;
                var x = -(battleMap.Size[0] - 1) / 2.0f;
                LeftUpCorner = new Vector3(x * TileSize.x  + Slope * y, y * TileSize.y);
            }
            
            for (int i = 0; i < battleMap.Tiles.Count; i++)
            {
                var tile = battleMap.Tiles[i];
                var pos = battleMap.DeltaToPos(i);
                tile.Initialize(new Vector3(pos.x * TileSize.x + Slope * -pos.y, -pos.y * TileSize.y) + LeftUpCorner,
                    i);
            }
        }
    }
}
