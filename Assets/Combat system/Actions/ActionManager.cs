using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CombatSystem.Entities
{
    public abstract class ActionManager : MonoBehaviour
    {
        public virtual string ID => "Not Implemented";

        public abstract bool SelectAction(Vector2Int Position);

        public abstract void ResetTurn();

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