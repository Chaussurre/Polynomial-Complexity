using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public abstract class Action
    {
        public abstract void Preview();
        public abstract void Apply();
        public abstract void Cancel();
    }
}
