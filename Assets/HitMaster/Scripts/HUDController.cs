using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HM {
    public class HUDController : MonoBehaviour {

#pragma warning disable 0649
        [SerializeField] private TMPro.TextMeshProUGUI _timerLabel;
        [SerializeField] private Image _reloadImage;
#pragma warning restore 0649

        private void Awake() {
            var player = FindObjectOfType<PlayerController>(true);
            if (player != null) {
                player.OnReloadProgress += HandleOnReloadProgress;
            }
        }

        private void Start() {
            GameManager.Instance.Timer.OnValueChanged += HandleOnTimerValueChanged;
            HandleOnTimerValueChanged(GameManager.Instance.Timer.TimeLeft);
        }

        private void HandleOnTimerValueChanged(float value) {
            _timerLabel.text = Mathf.CeilToInt(value).ToString();
        }

        private void HandleOnReloadProgress(float progress) {
            _reloadImage.fillAmount = progress;
        }
    }
}