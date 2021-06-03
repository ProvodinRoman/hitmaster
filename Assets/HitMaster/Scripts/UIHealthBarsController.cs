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
        private Dictionary<UIHealthBarOverHead, EntityController> _activeBars = new Dictionary<UIHealthBarOverHead, EntityController>();
        private Camera _camera;

        public void SetHealthBarsTo(List<EntityController> entities) {
            foreach (var activeBar in _activeBars) {
                activeBar.Key.gameObject.SetActive(false);
                _hpBarPool.ReturnToPool(activeBar.Key);
            }
            _activeBars.Clear();

            foreach (var entity in entities) {
                var bar = _hpBarPool.Get();
                bar.transform.SetParent(transform);
                bar.Bind(entity.Entity, _hpBarPool);
                bar.gameObject.SetActive(true);
                _activeBars.Add(bar, entity);
            }
        }

        private void Awake() {
            _hpBarPool = new ComponentPool<UIHealthBarOverHead>(_hpBarPrefab);
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