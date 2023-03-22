using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem.Selection
{
    [CreateAssetMenu(fileName = "Global Selection Filter", menuName = "Combat System/Selection Filter/Global")]
    public class GlobalSelectionFilter : SelectionTileFilter
    {
        protected override bool FilterTile(SelectionTile _, Vector2Int ___)
        {
            return true;
        }
    }
}
