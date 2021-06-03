using System;
using System.Collections;
using UnityEngine;

namespace HM {
    public class ThrowingWeapon : MonoBehaviour {
        public static event Action<ThrowingWeapon> OnLifeTimeOver;

#pragma warning disable 0649
        [SerializeField] private Collider _collider;
        [SerializeField] private float _lifetime = 10f;
        [SerializeField] private int _damage = 20;
#pragma warning restore 0649

        private Coroutine _lifeTimeRoutine = null;
        private bool _damageDelt = false;

        public void Throw(Vector3 worldPos, float speed) {
            transform.SetParent(null, true);
            transform.LookAt(worldPos);
            _collider.enabled = true;

            var rigidbody = _collider.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = false;
            rigidbody.AddForce((worldPos - transform.position).normalized * speed, ForceMode.Impulse);
            _lifeTimeRoutine = StartCoroutine(LifeTimeRoutine(_lifetime));
        }

        private IEnumerator LifeTimeRoutine(float lifetime) {
            yield return new WaitForSeconds(lifetime);
            OnLifeTimeOver?.Invoke(this);
            _lifeTimeRoutine = null;
        }

        private void OnDisable() {
            _damageDelt = false;

            if (_lifeTimeRoutine != null) {
                StopCoroutine(_lifeTimeRoutine);
            }

            OnLifeTimeOver?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other) {
            var enemy = other.GetComponentInParent<EntityController>();
            if (enemy != null && !_damageDelt) {
                _damageDelt = true;
                enemy.Entity.Health -= _damage;
            }

            transform.SetParent(other.transform, true);

            var rigidbody = _collider.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
            _collider.enabled = false;
        }
    }
}