using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;

//[CreateAssetMenu(fileName = "Selection Layer", menuName = "Battle System/Selection Layer", order = 1000)]

enum CombatEntityFilter
{
    NoFilter,
    IsEmpty,
    HasEntity,
    HasTargetable,
}
enum ContainItselfFilter
{
    NoFilter,
    AlwaysContainItself,
    NeverContainItself,
}

public abstract class SelectionLayerFilter : ScriptableObject
{
    [SerializeField] private CombatEntityFilter CombatEntityFilter;
    [SerializeField] private ContainItselfFilter ContainItselfFilter;
    public bool Filter(BattleMap Map, Vector2Int Origin, Vector2Int Tile)
    {
        if (Origin == Tile && ContainItselfFilter != ContainItselfFilter.NoFilter)
            return ContainItselfFilter == ContainItselfFilter.AlwaysContainItself;
        
        if (FilterTile(Map, Origin, Tile))
        {
            var hasEntity = Map.CombatEntitiesPos.TryGetValue(Tile, out var combatEntity);
            switch (CombatEntityFilter)
            {
                case CombatEntityFilter.IsEmpty:
                    return !hasEntity && FilterTile(Map, Origin, Tile);
                case CombatEntityFilter.HasEntity:
                    return hasEntity && FilterTile(Map, Origin, Tile);
                case CombatEntityFilter.HasTargetable:
                    return hasEntity && combatEntity.HealthStatus && Filter(Map, Origin, Tile);
                default:
                    return FilterTile(Map, Origin, Tile);
            }
        }

        return false;
    }

    protected abstract bool FilterTile(BattleMap Map, Vector2Int Origin, Vector2Int Tile);
}
