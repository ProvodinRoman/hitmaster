using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HM {
    public class UIMenuManager : MonoBehaviour {
        public event Action OnStartButtonClicked;

#pragma warning disable 0649
        [SerializeField] private GameObject _startScreen;
        [SerializeField] private Button _startButton;
#pragma warning restore 0649

        private void HandleOnStartButtonClicked() {
            _startScreen.SetActive(false);
            OnStartButtonClicked?.Invoke();
        }

        private void HandleOnGameOver() {
            _startScreen.SetActive(true);
        }

        private void Start() {
            _startButton.onClick.AddListener(HandleOnStartButtonClicked);
            GameManager.Instance.OnGameOver += HandleOnGameOver;
        }
    }
}