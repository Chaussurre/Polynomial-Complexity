using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem
{
    public class ActionBuffer : MonoBehaviour
    {
        #region Events

        public static UnityEvent<Action> AddAction = new();
        public static UnityEvent<int> PopActions = new();
        public static UnityEvent ClearActions = new();
        public static UnityEvent ApplyActions = new();

        public static UnityEvent<Action> OnActionAdded = new();

        #endregion
        
        private List<Action> WaitingActions = new();
        private Stack<Action> Historic = new();

        private void Awake()
        {
            AddAction.AddListener(AddActionMethod);
            PopActions.AddListener(PopActionsMethod);
            ClearActions.AddListener(ClearActionsMethod);
            ApplyActions.AddListener(ApplyActionsMethod);
        }

        public void ApplyActionsMethod()
        {
            foreach (var action in WaitingActions)
            {
                action.Apply();
                Historic.Push(action);
            }
            
            WaitingActions.Clear();
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

            //foreach (var action in WaitingActions)
            //    action.Preview();
        }

        private void AddActionMethod(Action action)
        {
            WaitingActions.Add(action);
            action.Preview();
            OnActionAdded.Invoke(action);
        }
    }
}
