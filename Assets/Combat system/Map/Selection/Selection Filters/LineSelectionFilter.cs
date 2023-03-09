using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    [CreateAssetMenu(fileName = "Line Selection Filter", menuName = "Combat System/Selection Filter/Line", order = 998)]
    public class LineSelectionFilter : SelectionLayerFilter
    {
        protected override bool FilterTile(SelectionLayer Layer, Vector2Int Tile)
        {
            return (Layer.Origin.x == Tile.x || Layer.Origin.y == Tile.y) &&
                 Mathf.Abs(Layer.Origin.x + Layer.Origin.y - Tile.x - Tile.y) <= Layer.Size;
        }
    }
}
