using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Actions
{
    public abstract class MapActionView
    {
        public abstract bool IsOver { get; }
        
        public abstract void Update(float deltaTime);

        public virtual void Start() {}

        public virtual void End() {}
    }
}
