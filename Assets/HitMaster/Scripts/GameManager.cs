using System;
using System.Collections.Generic;
using UnityEngine;
using HM.Extentions;

namespace HM {
    public class GameManager : MonoBehaviour {
        public event Action OnGameOver;

        public static GameManager Instance { get; private set; }

        public Timer Timer { get; private set; }

        public UIHealthBarsController UIHPBarsController => _uiHPBarsController;

#pragma warning disable 0649
        [SerializeField] private List<RoundController> _rounds;
        [SerializeField] private HUDController _HUDController;
        [SerializeField] private UIHealthBarsController _uiHPBarsController;
        [SerializeField] private UIMenuManager _menuManager;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private int _initialTime = 20;
        [Space]
        [SerializeField] private EntityController _enemyPrefab;
#pragma warning restore 0649

        private ComponentPool<EntityController> _enemiesPool;

        private int _currentRound = 0;

        public void MoveToNextRound() {
            _playerController.AllowShooting = false;

            _rounds[_currentRound].TransitToRound(
                        _rounds[_rounds.GetSafeClampedIndex(_currentRound + 1)],
                        _playerController,
                        Camera.main.transform,
                        3,
                        _enemiesPool,
                        () => {
                            _playerController.SetAnimatorIsRunning(false);
                            _playerController.AllowShooting = true;
                        },
                        () => Timer.IsPaused = false
                        );

            _currentRound = _rounds.GetSafeClampedIndex(_currentRound + 1);
        }

        public void RestartGame() {
            _rounds.ForEach(r => r.ClearRound(_enemiesPool));
            _uiHPBarsController.CleanUp();

            _playerController.Entity.Health = Entity.MaxHealth;

            Timer.TimeLeft = _initialTime;
            _currentRound = 0;
            _playerController.transform.position = _rounds[_rounds.GetSafeClampedIndex(_currentRound - 1)].GetLastPlayerWaypoint().pos;
            MoveToNextRound();
        }

        private void HandleOnTimerTicked(float timeLeft) {
            if (timeLeft <= 0) {
                _playerController.AllowShooting = false;
                _playerController.Entity.Health = 0;
                Timer.IsPaused = true;
                OnGameOver?.Invoke();
            }
        }

        private void HandleOnStartButtonClicked() {
            RestartGame();
        }

        private void HandleOnRoundCompleted(RoundController roundController) {
            MoveToNextRound();
        }

        private void HandleOnEntityDied(Entity entity) {
            if (_playerController.Entity != null && _playerController.Entity != entity) {
                Timer.TimeLeft += 5;
            }
        }

        private void Awake() {

            _enemiesPool = new ComponentPool<EntityController>(_enemyPrefab);

            _playerController.EnableColliders(false);
            _playerController.SetRigidbodyKinematicState(true);
            _playerController.EnableAnimator(true);

            var posData = _rounds[_rounds.GetSafeClampedIndex(_currentRound - 1)].GetLastPlayerWaypoint();
            _playerController.Init(new Entity(), posData.pos, posData.rotation.eulerAngles);

            Timer = new Timer(_initialTime, this);
            Timer.OnValueChanged += HandleOnTimerTicked;

            _menuManager.OnStartButtonClicked += HandleOnStartButtonClicked;

            RoundController.OnRoundCompleted += HandleOnRoundCompleted;
            Entity.OnDied += HandleOnEntityDied;

            Instance = this;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.N)) {
                MoveToNextRound();
            }
        }
    }
}