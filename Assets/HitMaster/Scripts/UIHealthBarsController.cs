using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class UIHealthBarsController : MonoBehaviour {
#pragma warning disable 0649
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UIHealthBarOverHead _hpBarPrefab;
#pragma warning restore 0649

        private ComponentPool<UIHealthBarOverHead> _hpBarPool;
        private ComponentPool<UIHealthBarOverHead> _HPBarPool {
            get {
                if(_hpBarPool == null) {
                    _hpBarPool = new ComponentPool<UIHealthBarOverHead>(_hpBarPrefab);
                }
                return _hpBarPool;
            }
        }
        private Dictionary<UIHealthBarOverHead, EntityController> _activeBars = new Dictionary<UIHealthBarOverHead, EntityController>();
        private Camera _camera;

        public void SetHealthBarsTo(List<EntityController> entities) {
            CleanUp();

            foreach (var entity in entities) {
                var bar = _HPBarPool.Get();
                bar.transform.SetParent(transform);
                bar.Bind(entity.Entity, _HPBarPool);
                bar.gameObject.SetActive(true);
                _activeBars.Add(bar, entity);
            }
        }

        public void CleanUp() {
            foreach (var activeBar in _activeBars) {
                activeBar.Key.gameObject.SetActive(false);
                _HPBarPool.ReturnToPool(activeBar.Key);
            }
            _activeBars.Clear();
        }

        private void Awake() {
            _camera = Camera.main;
        }

        private void LateUpdate() {
            foreach (var kvp in _activeBars) {
                Vector3 screenPos = _camera.WorldToScreenPoint(kvp.Value.HealthBarWorldPos);
                if (kvp.Key.transform.position != screenPos) {
                    kvp.Key.transform.position = screenPos;
                }
            }
        }

    }
}