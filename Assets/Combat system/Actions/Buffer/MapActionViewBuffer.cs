using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Actions
{
    public class MapActionViewBuffer : MonoBehaviour
    {
        #region Events

        public static UnityEvent<MapActionView> AddActionView = new();
        public static UnityEvent ResetActionViews = new();

        #endregion

        private Queue<MapActionView> Views = new();

        private void Awake()
        {
            AddActionView.AddListener(AddAction);
            ResetActionViews.AddListener(ResetActions);
            
            MapActionBuffer.PopActions.AddListener(ResetActions);
        }

        private void ResetActions(int _)
        {
            ResetActions();
        }

        private void ResetActions()
        {
            Views.Clear();
        }

        private void AddAction(MapActionView actionView)
        {
            if (Views.Count == 0)
                actionView.Start();
            Views.Enqueue(actionView);
        }

        private void Update()
        {
            while (Views.TryPeek(out var actionView) && actionView.IsOver)
            {
                Views.Dequeue().End();
                if(Views.TryPeek(out var next))
                    next.Start();
            }
            
            if(Views.TryPeek(out var current))
                current.Update(Time.deltaTime);
        }
    }
}
