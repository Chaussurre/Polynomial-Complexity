using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Entities;
using UnityEngine;

namespace CombatSystem.Actions
{
    public abstract class MapActionView
    {
        public abstract bool IsOver { get; }
        
        public abstract void Update(float deltaTime);

        public virtual void Start() {}

        public virtual void End() {}

        protected void SetAnimatorParameter(CombatEntityView entityView, string parameter, bool value,
            AnimatorControllerParameterType type)
        {
            
            foreach (var anim in entityView.Animators)
                if (anim.parameters.Any(x => x.name == parameter && x.type == type))
                    anim.SetBool(parameter, value);
        }
    }
}
