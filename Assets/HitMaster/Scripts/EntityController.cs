using System;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class EntityController : MonoBehaviour {

#pragma warning disable 0649
        [SerializeField] private List<Collider> _bodyColliders;
        [SerializeField] private Animator _animator;
#pragma warning restore 0649

        private Entity _entity;

        public void Init(Entity entity) {

        }

        public void EnableColliders(bool enable) {
            _bodyColliders.ForEach(c => c.enabled = enable);
        }

        public void EnableAnimator(bool enable) {
            _animator.enabled = enable;
        }

        public void SetRigidbodyKinematicState(bool isKinematic) {
            foreach (var collider in _bodyColliders) {
                if (collider.TryGetComponent<Rigidbody>(out var rigidBody)) {
                    rigidBody.isKinematic = isKinematic;
                }
            }
        }

        private void HandleOnHealthChanged(int before, int now) {

        }


        private void Awake() {
            
        }

        private void OnDisable() {
            if (_entity != null) {
                _entity.OnHealthChanged -= HandleOnHealthChanged;
            }
        }
    }
}