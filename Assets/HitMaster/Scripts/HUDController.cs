using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HM {
    public class HUDController : MonoBehaviour {
        public event Action OnStartButtonClicked;

#pragma warning disable 0649
        [SerializeField] private TMPro.TextMeshProUGUI _timerLabel;
        [SerializeField] private GameObject _startScreen;
        [SerializeField] private Button _startButton;
        [SerializeField] private Image _reloadImage;
#pragma warning restore 0649

        private void Awake() {
            _startButton.onClick.AddListener(HandleOnStartButtonClicked);
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

        private void HandleOnStartButtonClicked() {
            _startScreen.SetActive(false);
            OnStartButtonClicked?.Invoke();
        }

        private void HandleOnReloadProgress(float progress) {
            _reloadImage.fillAmount = progress;
        }
    }
}