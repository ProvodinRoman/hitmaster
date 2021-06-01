using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        public Timer Timer { get; private set; }

#pragma warning disable 0649
        [SerializeField] private List<RoundController> _rounds;
#pragma warning restore 0649


        private int _currentRound = 0;

        public void Continue() {

        }

        private void SpawnEnemies() {

        }

        private void Awake() {
            Instance = this;

            Timer = new Timer(20, this);
        }
    }
}