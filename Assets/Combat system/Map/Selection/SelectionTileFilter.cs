using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Map;
using UnityEngine;


namespace CombatSystem.Selection
{
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

    public abstract class SelectionTileFilter : ScriptableObject
    {
        [SerializeField] private CombatEntityFilter CombatEntityFilter;
        [SerializeField] private bool EntityFilterOnlyBlock;
        [SerializeField] private ContainItselfFilter ContainItselfFilter;
        public bool NeedPath;

        public virtual bool AllowReChoice => false;

        public bool Filter(SelectionTile tile, Vector2Int Tile, out bool BlockPath)
        {
            if (tile.Origin == Tile && ContainItselfFilter != ContainItselfFilter.NoFilter)
            {
                BlockPath = false;

                return ContainItselfFilter == ContainItselfFilter.AlwaysContainItself;
            }

            BlockPath = !FilterTile(tile, Tile);
            if (!BlockPath)
            {
                var hasEntity = BattleMap.CountEntity(Tile) > 0;
                
                switch (CombatEntityFilter)
                {
                    case CombatEntityFilter.IsEmpty:
                        BlockPath = hasEntity;
                        break;
                    case CombatEntityFilter.HasEntity:
                        BlockPath = !hasEntity;
                        break;
                    case CombatEntityFilter.HasTargetable:
                        var TargetableEntity = BattleMap.GetEntities(Tile).Any(x => x.HealthStatus);
                        BlockPath = !hasEntity || !TargetableEntity;
                        break;
                }

                if (hasEntity && EntityFilterOnlyBlock)
                    return true;
            }

            return !BlockPath;
        }

        protected abstract bool FilterTile(SelectionTile tile, Vector2Int Tile);
        
    }
}