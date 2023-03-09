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
        [SerializeField] private ContainItselfFilter ContainItselfFilter;
        public bool NeedPath;
        public bool UseSpeed;
        public int DefaultSpeed;

        public virtual bool AllowReChoice => false;

        public bool Filter(SelectionLayer Layer, Vector2Int Tile)
        {
            if (Layer.Origin == Tile && ContainItselfFilter != ContainItselfFilter.NoFilter)
                return ContainItselfFilter == ContainItselfFilter.AlwaysContainItself;

            if (FilterTile(Layer, Tile))
            {
                var hasEntity = Layer.Map.CombatEntitiesPos.TryGetValue(Tile, out var combatEntity);
                switch (CombatEntityFilter)
                {
                    case CombatEntityFilter.IsEmpty:
                        return !hasEntity;
                    case CombatEntityFilter.HasEntity:
                        return hasEntity;
                    case CombatEntityFilter.HasTargetable:
                        return hasEntity && combatEntity.HealthStatus;
                    default:
                        return true;
                }
            }

            return false;
        }

        protected abstract bool FilterTile(SelectionLayer Layer, Vector2Int Tile);
        
    }
}