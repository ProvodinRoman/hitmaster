using System;
using UnityEngine;

namespace HM {
    public class UserInputController : MonoBehaviour {
        public event Action<Vector2> OnTouchDetected;

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                OnTouchDetected?.Invoke(Input.mousePosition);
            }
        }
    }
}