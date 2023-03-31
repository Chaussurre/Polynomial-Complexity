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
        public SelectionTileFilter Range;
        public int RangeSize;

        public TargetPositionEffect Effect;
        
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
                RangeSize,
                OnSelect,
                OnCancel,
                Effect.OnHover));
        }

        private void OnSelect(SelectionTile selection, Vector2Int position)
        {
            Selections[repeatedCount] = position;
            repeatedCount++;
            
            if (repeatedCount == repeat)
                FinishedSelect(Origin);
            else
                AddSelectionLayer();
        }

        private void OnCancel(SelectionTile selection)
        {
            if (repeatedCount > 0)
                repeatedCount--;
        }

        protected override void Cast()
        {
            Effect.Cast(this);
        }
    }

    public abstract class TargetPositionEffect : AbilityEffect<TargetPositionAbility>
    {
        public SelectionTileFilter Zone;
        public int ZoneSize;

        [SerializeField] private bool HoverColor = true;
        [SerializeField] private Color TargetColor = Color.white;

        public override void Cast(TargetPositionAbility ability)
        {
            if (Zone)
                foreach (var position in ability.Selections
                             .Select(selection => new SelectionTile(
                                 selection,
                                 Zone,
                                 ZoneSize))
                             .SelectMany(zone => zone.Positions))
                    ApplyEffect(position);
            else
                foreach (var position in ability.Selections)
                    ApplyEffect(position);
        }

        protected abstract void ApplyEffect(Vector2Int position);

        public virtual void OnHover(SelectionTile selection, Vector2Int? position)
        {
            if (!position.HasValue || !selection.Positions.Contains(position.Value) || !HoverColor) return;

            if (Zone)
            {
                var zone = new SelectionTile(
                    position.Value, 
                    Zone,
                    ZoneSize);
                foreach (var zonePosition in zone.Positions)
                    BattleMap.Tiles[BattleMap.PosToDelta(zonePosition)]
                        .TileSelector
                        .SetColor(TargetColor);
            }
            else
            {
                BattleMap.Tiles[BattleMap.PosToDelta(position.Value)]
                    .TileSelector
                    .SetColor(TargetColor);
            }
        }
    }
}
