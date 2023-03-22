using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    [CreateAssetMenu(fileName = "Line Selection Filter", menuName = "Combat System/Selection Filter/Line")]
    public class LineSelectionFilter : SelectionTileFilter
    {
        protected override bool FilterTile(SelectionTile tile, Vector2Int Tile)
        {
            return (tile.Origin.x == Tile.x || tile.Origin.y == Tile.y) &&
                 Mathf.Abs(tile.Origin.x + tile.Origin.y - Tile.x - Tile.y) <= tile.Size;
        }
    }
}
