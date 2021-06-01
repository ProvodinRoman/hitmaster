using System;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class Entity {

        /// <summary>
        /// <before, now>
        /// </summary>
        public event Action<int, int> OnHealthChanged;

        private int _health;
        public int Health {
            get => _health;
            set {
                int before = _health;
                _health = value;
                OnHealthChanged?.Invoke(before, value);
            }
        }
    }
}