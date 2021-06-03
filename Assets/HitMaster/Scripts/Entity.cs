using System;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class Entity {

        /// <summary>
        /// <max, before, now>
        /// </summary>
        public event Action<int, int, int> OnHealthChanged;

        public static int MaxHealth = 100;

        private int _health;
        public int Health {
            get => _health;
            set {
                int before = _health;
                _health = value;
                OnHealthChanged?.Invoke(MaxHealth, before, value);
            }
        }

        public Entity() {
            _health = MaxHealth;
        }
    }
}