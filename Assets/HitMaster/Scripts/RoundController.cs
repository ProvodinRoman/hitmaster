using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HM {
    public class RoundController : MonoBehaviour {

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

        public void TransitToRound(RoundController nextRound, EntityController entity, Transform camera, 
                    int enemiesCount, ComponentPool<EntityController> enemiesPool, Action onCharacterAtFinish, Action onEnemiesJumpedIn) {
            entity.SetRigidbodyKinematicState(true);
            entity.EnableColliders(false);
            entity.EnableAnimator(true);

            entity.SetAnimatorIsRunning(true);

            Sequence cameraMoveSequence = DOTween.Sequence();
            camera.DOKill(true);
            cameraMoveSequence.Append(camera.DOMove(_cameraPos.position, .2f));
            cameraMoveSequence.Join( camera.DORotateQuaternion(_cameraPos.rotation, .2f));

            StartCoroutine(CharacterRunCoroutine(nextRound, entity.transform, 5f, enemiesCount, enemiesPool, onCharacterAtFinish,
                () => {
                    onEnemiesJumpedIn?.Invoke();
                    foreach (var e in _currentEnemies) {
                        e.EnableAnimator(false);
                        e.EnableColliders(false);
                        e.SetRigidbodyKinematicState(true);
                        e.gameObject.SetActive(false);
                        enemiesPool.ReturnToPool(e);
                    }
                    _currentEnemies.Clear();
                }));

            
            cameraMoveSequence.Append(camera.DOMove(nextRound._cameraPos.position, 3f));
            cameraMoveSequence.Join(camera.DORotateQuaternion(nextRound._cameraPos.rotation, 3f));
            cameraMoveSequence.Play();
        }

        private IEnumerator CharacterRunCoroutine(RoundController nextRound, Transform entity, float speed, 
                    int enemiesCount, ComponentPool<EntityController> enemiesPool, Action onCharacterAtFinish, Action onEnemiesJumpedIn) {

            foreach (var point in _playerWaypoints) {
                float distance = (entity.position - point.position).magnitude;
                entity.LookAt(point);
                yield return entity.DOMove(point.position, distance / speed).SetEase(Ease.Linear).WaitForCompletion();
                entity.rotation = point.rotation;
            }

            onCharacterAtFinish?.Invoke();
            yield return SpawnAndJumpInEnemies(nextRound, enemiesCount, enemiesPool);

            onEnemiesJumpedIn?.Invoke();
        }

        private IEnumerator SpawnAndJumpInEnemies(RoundController nextRound, int enemiesCount, ComponentPool<EntityController> enemiesPool) {

            for (int i = 0; i < enemiesCount && i < nextRound._enemiesSpawnPoints.Count && i < nextRound._enemiesAfterJumpPoints.Count; i++) {
                var enemy = enemiesPool.Get();
                enemy.SetRigidbodyKinematicState(true);
                enemy.EnableAnimator(false);
                enemy.EnableColliders(false);

                enemy.Init(new Entity(), nextRound._enemiesSpawnPoints[i].position, nextRound._enemiesSpawnPoints[i].eulerAngles);

                enemy.gameObject.SetActive(true);

                nextRound._currentEnemies.Add(enemy);

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
    }
}