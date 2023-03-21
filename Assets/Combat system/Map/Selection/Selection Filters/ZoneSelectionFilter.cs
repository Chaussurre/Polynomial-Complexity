using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    [CreateAssetMenu(fileName = "Zone Selection Filter", menuName = "Combat System/Selection Filter/Zone")]
    public class ZoneSelectionFilter : SelectionLayerFilter
    {
        public bool SquareZone;

        public override bool AllowReChoice => NeedPath && !SquareZone;

        protected override bool FilterTile(SelectionTile tile, Vector2Int Tile)
        {
            var dist = SquareZone 
                ? SquareDistance(Tile, tile.Origin)
                : CircleDistance(Tile, tile.Origin, tile);

            return dist <= tile.Size;
        }

        int CircleDistance(Vector2Int pos, Vector2Int Origin, SelectionTile tile)
        {
            if (!NeedPath)
            {
                var delta = pos - Origin;
                return Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
            }

            return tile.CountPath(pos);
        }

        int SquareDistance(Vector2Int Tile, Vector2Int Origin)
        {
            var delta = Tile - Origin;
            
            return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        }
    }
}
