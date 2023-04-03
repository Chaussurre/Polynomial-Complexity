using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Actions
{
    public abstract class MapAction
    {
        public abstract void Apply();
        public abstract void Cancel();
    }
}
