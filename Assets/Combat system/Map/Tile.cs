using System.Collections;
using System.Collections.Generic;
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
        public SpriteRenderer Renderer;

        public void SetPos(Vector3 position)
        {
            Renderer.sortingOrder = Mathf.FloorToInt(-position.y * 100);
            transform.position = position;
        }
    }
}
