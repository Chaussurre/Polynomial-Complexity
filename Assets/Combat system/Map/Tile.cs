using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Codice.Client.BaseCommands;
using CombatSystem.Entities;
using UnityEngine;

namespace CombatSystem.Map
{
    public enum TileType
    {
        Grass,
    }
    
    public class Tile : MonoBehaviour
    {
        public TileType Type;
        [SerializeField] private TileView TileView;

        public void Initialize(Vector3 position, int delta)
        {
            TileView.Initialize(delta);
            transform.position = position;
        }

        public void AddCombatEntity(CombatEntity entity)
        {
            TileView.CombatEntity = entity;
            TileView.RefreshCombatEntity();
        }

        public void RemoveCombatEntity()
        {
            TileView.CombatEntity = null;
        }
    }
}
