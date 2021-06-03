using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HM {
    public class UIHealthBarOverHead : MonoBehaviour {
#pragma warning disable 0649
        [SerializeField] private Image _healthBarFillImage;
#pragma warning restore 0649

        private Entity _boundEntity;
        private ComponentPool<UIHealthBarOverHead> _pool;

        public void Bind(Entity entity, ComponentPool<UIHealthBarOverHead> pool) {
            _pool = pool;

            if (_boundEntity != null) {
                _boundEntity.OnHealthChanged -= HandleOnHealthChanged;
            }

            _boundEntity = entity;
            entity.OnHealthChanged += HandleOnHealthChanged;
            HandleOnHealthChanged(Entity.MaxHealth, entity.Health, entity.Health);
        }

        private void HandleOnHealthChanged(int max, int before, int now) {

            if (now <= 0) {
                if (_boundEntity != null) {
                    _boundEntity.OnHealthChanged -= HandleOnHealthChanged;
                }

                gameObject.SetActive(false);
                _pool.ReturnToPool(this);
                return;
            }

            _healthBarFillImage.fillAmount = now / (float)max;
        }
    }
}