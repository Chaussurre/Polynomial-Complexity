using System.Collections.Generic;
using CombatSystem.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Actions
{
    public class MapActionBuffer : MonoBehaviour
    {
        #region Events

        public static UnityEvent<MapAction> AddAction = new();
        public static UnityEvent<int> PopActions = new();
        public static UnityEvent ClearActions = new();
        
        public static UnityEvent<MapAction> OnActionAdded = new();

        #endregion
        
        private List<MapAction> WaitingActions = new();
        private Stack<MapAction> Historic = new();

        private int ActionPassed;
        
        private void Awake()
        {
            AddAction.AddListener(AddActionMethod);
            PopActions.AddListener(PopActionsMethod);
            ClearActions.AddListener(ClearActionsMethod);
        }


        private void ClearActionsMethod()
        {
            PopActionsMethod(WaitingActions.Count);
        }

        private void PopActionsMethod(int number)
        {
            for (int i = 0; i < number && i < WaitingActions.Count; i++)
                WaitingActions[^(i + 1)].Cancel();

            WaitingActions.RemoveRange(WaitingActions.Count - number, number);

            ActionPassed = Mathf.Min(ActionPassed, WaitingActions.Count);
        }

        private void AddActionMethod(MapAction mapAction)
        {
            WaitingActions.Add(mapAction);
            mapAction.Apply();
            OnActionAdded.Invoke(mapAction);
        }
    }
}
