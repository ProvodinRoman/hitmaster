using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HM {
    public class PlayerController : EntityController {
        public event Action<float> OnReloadProgress; // fill 0-1

        public bool AllowShooting = false;

#pragma warning disable 0649
        [SerializeField] private Transform _rightHandWeaponHolder;
        [SerializeField] private ThrowingWeapon _knifePrefab;
        [SerializeField] private string _shootEventName = "Shoot";
#pragma warning restore 0649

        private ComponentPool<ThrowingWeapon> _knifesPool;
        private ThrowingWeapon _knifeInHand;

        private Vector3 _knifeTargetWorldPos;

        public override void Init(Entity entity, Vector3 worldPos, Vector3 eulerAngles) {
            base.Init(entity, worldPos, eulerAngles);
        }

        public void Shoot(Vector3 targetWorldPos) {
            if (AllowShooting) {
                AllowShooting = false;
                _knifeTargetWorldPos = targetWorldPos;
                _animator.SetTrigger("Shoot");
                StartCoroutine(ReloadRoutine(.5f));
            }
        }

        private IEnumerator ReloadRoutine(float reloadTime) {
            const float knifeAppearDuration = 0.5f;

            float startTime = Time.time;
            float finishTime = startTime + reloadTime;

            var delay = new WaitForEndOfFrame();

            while(Time.time < finishTime) {
                yield return delay;
                OnReloadProgress?.Invoke((Time.time - startTime) / (finishTime + knifeAppearDuration - startTime));
            }

            _knifeInHand = _knifesPool.Get();
            _knifeInHand.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _knifeInHand.transform.SetParent(_rightHandWeaponHolder);
            _knifeInHand.transform.SetPositionAndRotation(_rightHandWeaponHolder.position, _rightHandWeaponHolder.rotation);

            Vector3 eulerAngles = _knifeInHand.transform.localEulerAngles;
            eulerAngles.x = 180;
            _knifeInHand.transform.localEulerAngles = eulerAngles;

            _knifeInHand.transform.localScale = Vector3.zero;
            _knifeInHand.gameObject.SetActive(true);
            
            _knifeInHand.transform.DOScale(1, knifeAppearDuration).SetEase(Ease.InOutBack);

            while (Time.time < finishTime + knifeAppearDuration) {
                yield return delay;
                OnReloadProgress?.Invoke((Time.time - startTime) / (finishTime + knifeAppearDuration - startTime));
            }

            OnReloadProgress?.Invoke(1f);
            AllowShooting = true;
        }

        private void HandleOnTouchDetected(Vector2 screenPosInPixels) {
            if(Entity.Health <= 0 && !AllowShooting) {
                return;
            }

            var ray = Camera.main.ScreenPointToRay(screenPosInPixels);

            Vector3 targetWorldPos = default;
            if(Physics.Raycast(ray, out RaycastHit hitInfo, 100f)) {
                targetWorldPos = hitInfo.point;
            } else {
                targetWorldPos = ray.GetPoint(500);
            }

            Shoot(targetWorldPos);
        }

        private void HandleAnimatorEvent(string parameter) {
            if (_shootEventName.Equals(parameter)) {
                if (_knifeInHand != null && _knifeInHand.gameObject.activeInHierarchy) {
                    _knifeInHand.Throw(_knifeTargetWorldPos, 20f);
                }

                _knifeInHand = null;
            }
        }

        private void HandleOnKnifeLifeTimeOver(ThrowingWeapon knife) {
            knife.gameObject.SetActive(false);
            _knifesPool.ReturnToPool(knife);
        }

        private void Awake() {
            var inputController = FindObjectOfType<UserInputController>(true);
            if (inputController != null) {
                inputController.OnTouchDetected += HandleOnTouchDetected;
            }

            _knifesPool = new ComponentPool<ThrowingWeapon>(_knifePrefab);
            _knifeInHand = _rightHandWeaponHolder.GetComponentInChildren<ThrowingWeapon>();
            _animator.GetComponent<AnimatorEventsHandler>().OnEventFired += HandleAnimatorEvent;
            ThrowingWeapon.OnLifeTimeOver += HandleOnKnifeLifeTimeOver;
        }
    }
}