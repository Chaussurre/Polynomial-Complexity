using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    [CreateAssetMenu(fileName = "Zone Selection Filter", menuName = "Combat System/Selection Filter/Zone", order = 999)]
    public class ZoneSelectionFilter : SelectionLayerFilter
    {
        public bool NeedPath;
        public bool SquareZone;
        public int ZoneRadius;


        protected override bool FilterTile(MapSelectionManager Map, Vector2Int Origin, Vector2Int Tile)
        {
            var dist = SquareZone ? SquareDistance(Tile, Origin) : CircleDistance(Tile, Origin, Map);

            return dist <= ZoneRadius;
        }

        int CircleDistance(Vector2Int pos, Vector2Int Origin, MapSelectionManager Map)
        {
            if (!IsContinuous || !NeedPath)
            {
                var delta = pos - Origin;
                return Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
            }
            
            int count = 0;
            int current = Map.BattleMap.PosToDelta(pos);
            int prev = Map.PreviousVector[current];
            while (current != prev)
            {
                current = prev;
                prev = Map.PreviousVector[prev];
                count++;
            }
            return count;
        }

        int SquareDistance(Vector2Int Tile, Vector2Int Origin)
        {
            var delta = Tile - Origin;
            
            return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        }
    }
}
