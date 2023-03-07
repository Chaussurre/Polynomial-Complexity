using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using UnityEngine;

namespace CombatSystem
{
    [CreateAssetMenu(fileName = "Global Selection Filter", menuName = "Combat System/Selection Filter/Global", order = 1000)]
    public class GlobalSelectionFilter : SelectionLayerFilter
    {
        protected override bool FilterTile(MapSelectionManager _, Vector2Int __, Vector2Int ___)
        {
            return true;
        }
    }
}
