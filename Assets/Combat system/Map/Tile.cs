using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Codice.Client.BaseCommands;
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
    }
}
