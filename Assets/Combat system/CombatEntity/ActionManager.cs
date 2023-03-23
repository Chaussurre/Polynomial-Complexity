using UnityEngine;

namespace CombatSystem.Entities
{
    public abstract class ActionManager : MonoBehaviour
    {
        public abstract bool SelectAction(Vector2Int Position);

        public abstract void ResetTurn();

        public abstract void EndTurn();
    }
}