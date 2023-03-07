using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Events
{
    [Serializable]
    public class DataWatcher<T>
    {
        List<Func<T, T>> Modifiers;
        public UnityEvent<T> OnEvent;
        
        public DataWatcher(int capacity)
        {
            Modifiers = new List<Func<T, T>>(capacity);
        }

        public T WatchData(T Data)
        {
            Modifiers.ForEach(x => Data = x(Data));

            OnEvent?.Invoke(Data);
            return Data;
        }

        public void AddModifier(Func<T, T> modifier)
        {
            if (Modifiers.Count == Modifiers.Capacity)
                Debug.LogWarning($"Too many watchers on type {typeof(T)}");
            Modifiers.Add(modifier);
        }

        public void RemoveModifier(Func<T, T> modifier)
        {
            Modifiers.Remove(modifier);
        }
    }
}