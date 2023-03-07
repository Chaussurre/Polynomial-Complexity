using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class CombatEntity : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;
        
        public HealthStatus HealthStatus;

        public MovementManager MovementManager;
    }
}
