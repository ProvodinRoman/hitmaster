using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace HM {
    public class RoundController : MonoBehaviour {
        public static event Action<RoundController> OnRoundCompleted;

#pragma warning disable 0649
        [SerializeField] private List<Transform> _enemiesSpawnPoints;
        [SerializeField] private List<Transform> _enemiesAfterJumpPoints;
        [SerializeField] private List<Transform> _playerWaypoints;
        [SerializeField] private Transform _cameraPos;

        [Space]
        [SerializeField] private int _rewardSeconds;
#pragma warning restore 0649

        private List<EntityController> _currentEnemies = new List<EntityController>();

        public (Vector3 pos, Quaternion rotation) GetLastPlayerWaypoint() {
            return (_playerWaypoints[_playerWaypoints.Count - 1].position, _playerWaypoints[_playerWaypoints.Count - 1].rotation);
        }

        public void TransitToRound(RoundController nextRound, EntityController playerEntity, Transform camera, 
                    int enemiesCount, ComponentPool<EntityController> enemiesPool, Action onCharacterAtFinish, Action onEnemiesJumpedIn) {
            playerEntity.SetRigidbodyKinematicState(true);
            playerEntity.EnableColliders(false);
            playerEntity.EnableAnimator(true);

            playerEntity.SetAnimatorIsRunning(true);

            Sequence cameraMoveSequence = DOTween.Sequence();
            camera.DOKill(true);
            cameraMoveSequence.Append(camera.DOMove(_cameraPos.position, .2f));
            cameraMoveSequence.Join( camera.DORotateQuaternion(_cameraPos.rotation, .2f));

            StartCoroutine(CharacterRunCoroutine(nextRound, playerEntity.transform, 5f, enemiesCount, enemiesPool, onCharacterAtFinish,
                () => {
                    onEnemiesJumpedIn?.Invoke();
                    ClearRound(enemiesPool);
                }));

            
            cameraMoveSequence.Append(camera.DOMove(nextRound._cameraPos.position, 3f));
            cameraMoveSequence.Join(camera.DORotateQuaternion(nextRound._cameraPos.rotation, 3f));
            cameraMoveSequence.Play();
        }

        public void ClearRound(ComponentPool<EntityController> enemiesPool) {
            foreach (var e in _currentEnemies) {
                e.EnableAnimator(false);
                e.EnableColliders(false);
                e.SetRigidbodyKinematicState(true);
                e.gameObject.SetActive(false);
                e.Entity.OnHealthChanged -= HandleOnEnemyHealthChanged;
                enemiesPool.ReturnToPool(e);
            }
            _currentEnemies.Clear();
        }

        private IEnumerator CharacterRunCoroutine(RoundController nextRound, Transform playerEntity, float speed, 
                    int enemiesCount, ComponentPool<EntityController> enemiesPool, Action onCharacterAtFinish, Action onEnemiesJumpedIn) {

            foreach (var point in _playerWaypoints) {
                float distance = (playerEntity.position - point.position).magnitude;
                playerEntity.LookAt(point);
                yield return playerEntity.DOMove(point.position, distance / speed).SetEase(Ease.Linear).WaitForCompletion();
                playerEntity.rotation = point.rotation;
            }

            onCharacterAtFinish?.Invoke();
            yield return SpawnAndJumpInEnemies(nextRound, enemiesCount, enemiesPool);

            onEnemiesJumpedIn?.Invoke();
        }

        private IEnumerator SpawnAndJumpInEnemies(RoundController nextRound, int enemiesCount, ComponentPool<EntityController> enemiesPool) {

            for (int i = 0; i < enemiesCount && i < nextRound._enemiesSpawnPoints.Count && i < nextRound._enemiesAfterJumpPoints.Count; i++) {
                var enemy = enemiesPool.Get();
                enemy.SetRigidbodyKinematicState(true);
                enemy.EnableAnimator(true);
                enemy.EnableColliders(false);

                enemy.Init(new Entity(), nextRound._enemiesSpawnPoints[i].position, nextRound._enemiesSpawnPoints[i].eulerAngles);

                enemy.gameObject.SetActive(true);

                nextRound._currentEnemies.Add(enemy);
                enemy.Entity.OnHealthChanged += nextRound.HandleOnEnemyHealthChanged;

                yield return null;
            }

            Sequence s = DOTween.Sequence();

            for (int i = 0; i < nextRound._currentEnemies.Count && i < nextRound._enemiesAfterJumpPoints.Count; i++) {

                float jumpForce = UnityEngine.Random.Range(1.5f, 3f);
                float duration = UnityEngine.Random.Range(.5f, 1f);

                float delay = UnityEngine.Random.Range(.1f, .4f);

                var curEnemy = nextRound._currentEnemies[i];
                var tween = curEnemy.transform.DOJump(nextRound._enemiesAfterJumpPoints[i].position, jumpForce, 1, duration)
                    .SetEase(Ease.InCubic);
                tween.Join(curEnemy.transform.DORotateQuaternion(nextRound._enemiesAfterJumpPoints[i].rotation, duration));


                tween.OnComplete(() => {
                    curEnemy.SetRigidbodyKinematicState(true);
                    curEnemy.EnableAnimator(true);
                    curEnemy.EnableColliders(true);
                });
                s.Join(tween);
            }

            GameManager.Instance.UIHPBarsController.SetHealthBarsTo(nextRound._currentEnemies);

            yield return s.Play().WaitForCompletion();
        }

        private void HandleOnEnemyHealthChanged(int max, int before, int now) {
            int deadEnemies = _currentEnemies.Count(e => e.Entity.Health <= 0);

            if (deadEnemies >= _currentEnemies.Count) {
                OnRoundCompleted?.Invoke(this);
            }
        }
    }
}