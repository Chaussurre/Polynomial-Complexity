using CombatSystem.Entities;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem.Map
{
    
    public class Tile : MonoBehaviour
    {
        public TileView TileView;
        public TileSelector TileSelector;

        public void Initialize(Vector3 position, int delta)
        {
            TileView.Initialize(delta);
            transform.position = position;
        }

        public void AddCombatEntity(CombatEntity entity)
        {
            TileView.CombatEntity = entity;
        }

        public void RemoveCombatEntity()
        {
            TileView.CombatEntity = null;
        }
    }
}
