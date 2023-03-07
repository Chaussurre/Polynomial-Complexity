using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    [CreateAssetMenu(fileName = "Zone Selection Filter", menuName = "Combat System/Selection Filter/Zone", order = 999)]
    public class ZoneSelectionFilter : SelectionLayerFilter
    {
        public bool SquareZone;
        public int ZoneRadius;


        protected override bool FilterTile(BattleMap _, Vector2Int Origin, Vector2Int Tile)
        {
            var delta = Origin - Tile;
            var dist = SquareZone ? SquareDistance(delta) : CircleDistance(delta);

            return dist <= ZoneRadius;
        }

        int CircleDistance(Vector2Int delta)
        {
            return Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
        }

        int SquareDistance(Vector2Int delta)
        {
            return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        }
    }
}
