using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    [CreateAssetMenu(fileName = "Line Selection Filter", menuName = "Combat System/Selection Filter/Line", order = 998)]
    public class LineSelectionFilter : SelectionLayerFilter
    {
        public int Distance;
        
        protected override bool FilterTile(BattleMap Map, Vector2Int Origin, Vector2Int Tile)
        {
            return (Origin.x == Tile.x || Origin.y == Tile.y) &&
                 Mathf.Abs(Origin.x + Origin.y - Tile.x - Tile.y) <= Distance;
        }
    }
}
