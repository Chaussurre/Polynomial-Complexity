using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CombatSystem.Entities
{
    public abstract class ActionManager : MonoBehaviour
    {
        /// <summary>
        /// The ID from which the TurnManager calls the manager
        /// </summary>
        public virtual string ID => "Not Implemented";

        /// <summary>
        /// Called when the current step of the turn concerns this manager
        /// </summary>
        /// <param name="Position">The position of the entity at the time of this step</param>
        /// <returns>True iff the manager is available</returns>
        public abstract bool SelectAction(Vector2Int Position);

        /// <summary>
        /// Reset all infos from this manager as if a new turn has begun.
        /// </summary>
        public abstract void ResetTurn();
        
        /// <summary>
        /// Called once the turn is over and all actions are confirmed.
        /// </summary>
        public abstract void EndTurn();
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(ActionManager), editorForChildClasses: true), CanEditMultipleObjects]
    public class ActionManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var manager = target as ActionManager;

            if (manager)
            {
                GUILayout.Label("ID :");
                GUILayout.TextField(manager.ID);
                GUILayout.Space(10);
            }

            base.OnInspectorGUI();
        }
    }
    
#endif
}