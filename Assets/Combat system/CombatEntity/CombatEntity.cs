using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CombatSystem.Entities
{
    public class CombatEntity : MonoBehaviour
    {
        public CombatEntityView View; 
        
        public HealthStatus HealthStatus;
    }
}
