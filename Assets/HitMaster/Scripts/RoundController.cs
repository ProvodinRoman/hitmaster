using System;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class RoundController : MonoBehaviour {
#pragma warning disable 0649
        [SerializeField] private List<Transform> _playerWaypoints;
        [SerializeField] private List<Transform> _enemiesSpawnPoints;
        [SerializeField] private List<Transform> _enemiesAfterJumpPoints;
        [SerializeField] private Transform _cameraPos;

        [Space]
        [SerializeField] private int _rewardSeconds;
#pragma warning restore 0649

        public void TransitToRound(EntityController entity, Action onComplete) {
            entity.SetRigidbodyKinematicState(true);
            entity.EnableColliders(false);
            entity.EnableAnimator(true);
        }
    }
}