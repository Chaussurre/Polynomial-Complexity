using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Map;

namespace CombatSystem.Selection
{
    public class TileSelector : MonoBehaviour
    {
        [SerializeField] private TileView TileView;
        
        public Collider2D Collider;
        public BattleMap BattleMap { get; set; }
        
        private void Start()
        {
            BattleMap = GetComponentInParent<BattleMap>();
        }

        public bool isOverMe(Vector2 pos)
        {
            return Collider.OverlapPoint(pos);
        }

        public void Hover()
        {
            TileView.Hover();
        }

        public void UnHover()
        {
            TileView.ResetSizeAndColor();
        }

        public void Select()
        {
            TileView.Select();
        }

        public void Unselect()
        {
            TileView.ResetSizeAndColor();
        }
    }
}
