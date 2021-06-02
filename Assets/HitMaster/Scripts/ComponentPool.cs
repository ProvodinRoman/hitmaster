using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class ComponentPool<T> where T : Component {
        private T _prefab;
        private Stack<T> _availableObjects = new Stack<T>();

        public ComponentPool(T prefab){
            _prefab = prefab;
        }

        public T Get() {
            T res = null;

            if(_availableObjects.Count > 0) {
                res = _availableObjects.Pop();
            } else {
                res = MonoBehaviour.Instantiate(_prefab);
            }

            return res;
        }

        public void ReturnToPool(T obj) {
            _availableObjects.Push(obj);
        }
    }
}