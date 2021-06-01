using System;
using System.Collections;
using UnityEngine;

namespace HM {
    public class Timer {
        public event Action<float> OnValueChanged;

        private float _timeLeft;
        public float TimeLeft {
            get => _timeLeft;
            set {
                _timeLeft = value;
                OnValueChanged?.Invoke(value);
            }
        }

        private bool _isPaused;
        public bool IsPaused {
            get => _isPaused;
            set {
                _isPaused = value;

                if (value) {
                    if (_coroutine != null) {
                        _coroutineHolder.StopCoroutine(_coroutine);
                        _coroutine = null;
                    }
                } else {
                    if (_coroutine == null) {
                        _coroutine = _coroutineHolder.StartCoroutine(CoroutineMethod(_tickDelay));
                    }
                }
            }
        }

        private MonoBehaviour _coroutineHolder;
        private Coroutine _coroutine;
        private const float _tickDelay = 0.2f;

        public Timer(int initialTimeLeft,  MonoBehaviour coroutineHolder, bool stayPaused = true) {
            _coroutineHolder = coroutineHolder;
            TimeLeft = initialTimeLeft;
            _isPaused = stayPaused;

            if (!stayPaused) {
                _coroutine = _coroutineHolder.StartCoroutine(CoroutineMethod(_tickDelay));
            }
        }

        private IEnumerator CoroutineMethod(float delaySeconds) {
            var delay = new WaitForSeconds(delaySeconds);

            while (TimeLeft > 0) {
                float timeBefore = Time.time;
                yield return delay;
                float timeAfter = Time.time;
                TimeLeft -= timeAfter - timeBefore;
            }
        }
    }
}
