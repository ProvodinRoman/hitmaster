using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class EntityController : MonoBehaviour {

        public Entity Entity { get; private set; }
        public Vector3 HealthBarWorldPos => _healthBarPos.position;

#pragma warning disable 0649
        [SerializeField] private List<Collider> _bodyColliders;
        [SerializeField] private Transform _healthBarPos;
        [SerializeField] protected Animator _animator;
#pragma warning restore 0649

        public virtual void Init(Entity entity, Vector3 worldPos, Vector3 eulerAngles) {
            Entity = entity;
            Entity.OnHealthChanged += HandleOnHealthChanged;
            transform.position = worldPos;
            transform.eulerAngles = eulerAngles;
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

        public void SetAnimatorIsRunning(bool value) {
            _animator.SetBool("IsRunning", value);
        }

        private void HandleOnHealthChanged(int max, int before, int now) {
            if (now <= 0) {
                EnableAnimator(false);
                EnableColliders(true);
                SetRigidbodyKinematicState(false);
            }
        }

        private void OnDisable() {
            if (Entity != null) {
                Entity.OnHealthChanged -= HandleOnHealthChanged;
            }
        }
    }
}