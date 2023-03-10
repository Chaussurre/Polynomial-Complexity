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

        public override bool AllowReChoice => NeedPath && !SquareZone;

        protected override bool FilterTile(SelectionLayer Layer, Vector2Int Tile)
        {
            var dist = SquareZone 
                ? SquareDistance(Tile, Layer.Origin)
                : CircleDistance(Tile, Layer.Origin, Layer);

            return dist <= Layer.Size;
        }

        int CircleDistance(Vector2Int pos, Vector2Int Origin, SelectionLayer Layer)
        {
            if (!NeedPath)
            {
                var delta = pos - Origin;
                return Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
            }

            return Layer.CountPath(pos);
        }

        int SquareDistance(Vector2Int Tile, Vector2Int Origin)
        {
            var delta = Tile - Origin;
            
            return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        }
    }
}
