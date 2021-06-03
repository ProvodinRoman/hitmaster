using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HM.Extentions;

namespace HM {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        public Timer Timer { get; private set; }

        public UIHealthBarsController UIHPBarsController => _uiHPBarsController;

#pragma warning disable 0649
        [SerializeField] private List<RoundController> _rounds;
        [SerializeField] private HUDController _HUDController;
        [SerializeField] private UIHealthBarsController _uiHPBarsController;
        [SerializeField] private EntityController _playerController;
        [Space]
        [SerializeField] private EntityController _enemyPrefab;
#pragma warning restore 0649

        private ComponentPool<EntityController> _enemiesPool;

        private int _currentRound = 0;

        public void MoveToNextRound() {
            _rounds[_currentRound].TransitToRound(
                        _rounds[_rounds.GetSafeClampedIndex(_currentRound + 1)],
                        _playerController,
                        Camera.main.transform,
                        3,
                        _enemiesPool,
                        () => _playerController.SetAnimatorIsRunning(false),
                        () => Timer.IsPaused = false
                        );

            _currentRound = _rounds.GetSafeClampedIndex(_currentRound + 1);
        }

        private void HandleOnPlayerReady() {

        }

        private void HandleOnTimerTicked(float timeLeft) {
            if (timeLeft <= 0) {

            }
        }

        private void HandleOnStartButtonClicked() {
            MoveToNextRound();
        }

        private void Awake() {

            _enemiesPool = new ComponentPool<EntityController>(_enemyPrefab);

            _playerController.EnableColliders(false);
            _playerController.SetRigidbodyKinematicState(true);
            _playerController.EnableAnimator(true);

            var posData = _rounds[_rounds.GetSafeClampedIndex(_currentRound - 1)].GetLastPlayerWaypoint();
            _playerController.Init(new Entity(), posData.pos, posData.rotation.eulerAngles);

            Timer = new Timer(20, this);
            Timer.OnValueChanged += HandleOnTimerTicked;

            _HUDController.OnStartButtonClicked += HandleOnStartButtonClicked;

            foreach (var round in _rounds) {

            }

            Instance = this;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.N)) {
                MoveToNextRound();
            }
        }

    }
}