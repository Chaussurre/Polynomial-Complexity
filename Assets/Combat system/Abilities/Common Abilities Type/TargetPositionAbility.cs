using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEditorInternal;
using UnityEngine;

namespace CombatSystem.Abilities
{
    public class TargetPositionAbility : Ability
    {
        public SelectionLayerFilter Range;
        public int RangeSize;
        public SelectionLayerFilter Zone;
        public int ZoneSize;

        public TargetPositionEffect Effect;

        [SerializeField] private Color TargetColor = Color.white;

        [SerializeField] private int repeat = 1;
        
        [HideInInspector] public Vector2Int Origin;
        [HideInInspector] public List<Vector2Int> Selections = new();
        private int repeatedCount;
        
        public override void Start()
        {
            base.Start();
            for (int i = 0; i < repeat; i++)
                Selections.Add(Vector2Int.zero);
        }

        public override void Select(Vector2Int Position)
        {
            Origin = Position;
            repeatedCount = 0;
            AddSelectionLayer();
        }

        private void AddSelectionLayer()
        {
            SelectionStackManager.AddLayer?.Invoke(new SelectionTile(
                Origin,
                Range,
                OnSelect,
                OnCancel,
                OnHover,
                RangeSize));
        }

        private void OnSelect(SelectionTile selection, Vector2Int position)
        {
            Selections[repeatedCount] = position;
            repeatedCount++;
            
            if (repeatedCount == repeat)
                Cast();
            else
                AddSelectionLayer();
        }

        private void OnCancel(SelectionTile selection)
        {
            if (repeatedCount > 0)
                repeatedCount--;
        }

        private void OnHover(SelectionTile selection, Vector2Int? position)
        {
            if (!position.HasValue || !selection.Positions.Contains(position.Value)) return;

            if (Zone)
            {
                var zone = new SelectionTile(
                    position.Value,
                    Zone,
                    null, null, null,
                    ZoneSize);
                foreach (var zonePosition in zone.Positions)
                    BattleMap.Tiles[BattleMap.PosToDelta(zonePosition)]
                        .TileSelector
                        .SetColor(TargetColor);
            }
            else
                BattleMap.Tiles[BattleMap.PosToDelta(position.Value)]
                    .TileSelector.SetColor(TargetColor);
        }

        public override void Cast()
        {
            Effect.Cast(this);
            SelectionStackManager.ClearStack?.Invoke();
        }
    }

    public abstract class TargetPositionEffect : AbilityEffect<TargetPositionAbility>
    {
        public override void Cast(TargetPositionAbility ability)
        {
            if (ability.Zone)
                foreach (var position in ability.Selections
                             .Select(selection => new SelectionTile(
                                 selection,
                                 ability.Zone,
                                 null,
                                 null,
                                 null,
                                 ability.ZoneSize))
                             .SelectMany(zone => zone.Positions))
                    ApplyEffect(position);
            else
                foreach (var position in ability.Selections)
                    ApplyEffect(position);
        }

        protected abstract void ApplyEffect(Vector2Int position);
    }
}
