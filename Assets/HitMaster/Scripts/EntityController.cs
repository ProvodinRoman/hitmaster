using System;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class EntityController : MonoBehaviour {

#pragma warning disable 0649
        [SerializeField] private List<Collider> _bodyColliders;
        [SerializeField] private Transform _rightHandWeaponHolder;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649

        private Entity _entity;
        private bool _readyToShoot = false;

        public void Init(Entity entity, Vector3 worldPos, Vector3 eulerAngles) {
            _entity = entity;
            transform.position = worldPos;
            transform.eulerAngles = eulerAngles;
            _readyToShoot = true;
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

        public void Shoot(Vector3 targetWorldPos) {
            _animator.SetTrigger("Shoot");
        }

        public void Equip(Transform obj) {
            obj.SetParent(_rightHandWeaponHolder, false);
            obj.SetPositionAndRotation(_rightHandWeaponHolder.position, _rightHandWeaponHolder.rotation);
            obj.localScale = Vector3.zero;
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