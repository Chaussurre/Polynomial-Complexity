using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Events;
using UnityEngine;

namespace CombatSystem.Entities
{
    public struct changeHP
    {
        public int CurrentHp { get; private set; }
        public int delta;
        public int MaxHp { get; private set; }
        public float multiplier;

        public changeHP(int currentHp, int delta, int maxHp)
        {
            CurrentHp = currentHp;
            this.delta = delta;
            MaxHp = maxHp;
            multiplier = 1;
        }
    }
    
    public class HealthStatus : MonoBehaviour
    {
        public int MaxHp;
        public int Hp { get; private set; }

        public DataWatcher<changeHP> DealDamageWatcher = new DataWatcher<changeHP>(10);
        public DataWatcher<changeHP> HealHpWatcher = new DataWatcher<changeHP>(10);
        
        private void Start()
        {
            Hp = MaxHp;
        }

        public void DealDamage(int damage)
        {
            var changeHp =  DealDamageWatcher.WatchData(new changeHP(Hp, -damage, MaxHp));

            Hp += Mathf.RoundToInt(changeHp.delta * changeHp.multiplier);

            Hp = Mathf.Clamp(Hp, 0, MaxHp);
        }

        public void Heal(int heal)
        {
            var changeHp = HealHpWatcher.WatchData(new changeHP (Hp, heal, MaxHp));

            Hp += Mathf.RoundToInt(changeHp.delta * changeHp.multiplier);

            Hp = Mathf.Clamp(Hp, 0, MaxHp);
        }
    }
}
