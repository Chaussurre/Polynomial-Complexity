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

    public abstract class SelectionLayerFilter : ScriptableObject
    {
        [SerializeField] private CombatEntityFilter CombatEntityFilter;
        [SerializeField] private bool EntityFilterOnlyBlock;
        [SerializeField] private ContainItselfFilter ContainItselfFilter;
        public bool NeedPath;
        public bool UseSpeed;
        public int DefaultSpeed;

        public virtual bool AllowReChoice => false;

        public bool Filter(SelectionLayer Layer, Vector2Int Tile, out bool BlockPath)
        {
            if (Layer.Origin == Tile && ContainItselfFilter != ContainItselfFilter.NoFilter)
            {
                BlockPath = false;

                return ContainItselfFilter == ContainItselfFilter.AlwaysContainItself;
            }

            BlockPath = !FilterTile(Layer, Tile);
            if (!BlockPath)
            {
                var hasEntity = BattleMap.HasEntity(Tile, out var combatEntity);
                switch (CombatEntityFilter)
                {
                    case CombatEntityFilter.IsEmpty:
                        BlockPath = hasEntity;
                        break;
                    case CombatEntityFilter.HasEntity:
                        BlockPath = !hasEntity;
                        break;
                    case CombatEntityFilter.HasTargetable:
                        BlockPath = !hasEntity || !combatEntity.HealthStatus;
                        break;
                }

                if (hasEntity && EntityFilterOnlyBlock)
                    return true;
            }

            return !BlockPath;
        }

        protected abstract bool FilterTile(SelectionLayer Layer, Vector2Int Tile);
        
    }
}