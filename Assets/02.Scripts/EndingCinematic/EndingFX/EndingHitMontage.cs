using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace EndingFX
{
    public sealed class EndingHitMontage : MonoBehaviour
    {
        [FormerlySerializedAs("slashPool")]
        [SerializeField] private FireSlashPool _slashPool;
        [FormerlySerializedAs("hitBurst")]
        [SerializeField] private ImpactBurst _hitBurst;
        [FormerlySerializedAs("finalBurst")]
        [SerializeField] private ImpactBurst _finalBurst;
        [FormerlySerializedAs("clickInterval")]
        [SerializeField, Min(0.01f)] private float _clickInterval = 0.08f;
        [FormerlySerializedAs("fullIntensityHits")]
        [SerializeField, Min(1)] private int _fullIntensityHits = 24;
        [FormerlySerializedAs("slashScale")]
        [SerializeField] private float _slashScale = 1f;
        [FormerlySerializedAs("finalSlashScale")]
        [SerializeField] private float _finalSlashScale = 1.6f;
        [FormerlySerializedAs("scatterRadius")]
        [SerializeField] private float _scatterRadius = 0.6f;
        public event Action<float> HitLanded;
        public bool IsAcceptingInput { get; private set; }
        public int HitsLanded { get; private set; }
        public bool IsConfigured => _slashPool != null && _hitBurst != null && _finalBurst != null;
        private Transform _target;
        private float _nextClick;
        private int _slashIndex;
        private bool _running;
        private bool _finalPeaked;
        private FireSlashEffect _finalSlash;
        private static readonly float[] Angles = { 20, -35, 5, 160, -20, 40, 185, -50 };

        private void OnEnable()
        {
            if (_slashPool != null)
            {
                _slashPool.SlashPeaked += HandlePeak;
            }
        }

        private void OnDisable()
        {
            Stop();
            if (_slashPool != null)
            {
                _slashPool.SlashPeaked -= HandlePeak;
            }
        }

        private void Update()
        {
            if (IsAcceptingInput && Input.GetMouseButtonDown(0))
            {
                TryClick();
            }
        }

        public bool TryClick()
        {
            if (!IsAcceptingInput || _target == null || Time.unscaledTime < _nextClick)
            {
                return false;
            }

            _nextClick = Time.unscaledTime + _clickInterval;
            _slashPool.Spawn(_target.position + (Vector3)(UnityEngine.Random.insideUnitCircle * _scatterRadius),
                Angles[_slashIndex % Angles.Length], _slashIndex++ % 2 == 1, _slashScale);
            return true;
        }

        public IEnumerator PlayHits(Transform hitTarget, float duration)
        {
            Stop();
            if (!IsConfigured || hitTarget == null || !isActiveAndEnabled)
            {
                yield break;
            }

            _running = true;
            _target = hitTarget;
            HitsLanded = _slashIndex = 0;
            _nextClick = Time.unscaledTime;
            IsAcceptingInput = true;
            float end = Time.unscaledTime + Mathf.Max(0, duration);
            while (_running && _target != null && Time.unscaledTime < end)
            {
                yield return null;
            }

            IsAcceptingInput = false;
            // Finish pending ordinary hits before identifying the final slash.
            float drain = Time.unscaledTime + 0.6f;
            while (_running && Time.unscaledTime < drain)
            {
                yield return null;
            }
        }

        public IEnumerator PlayFinalHit(Transform hitTarget)
        {
            if (!_running || hitTarget == null)
            {
                yield break;
            }

            _finalPeaked = false;
            _finalSlash = _slashPool.Spawn(hitTarget.position, -25, false, _finalSlashScale);
            float deadline = Time.unscaledTime + 2f;
            while (_running && !_finalPeaked && Time.unscaledTime < deadline)
            {
                yield return null;
            }

            if (_running && !_finalPeaked)
            {
                _finalBurst.Play(hitTarget.position, -25, 1);
                HitLanded?.Invoke(1);
            }
        }

        private void HandlePeak(FireSlashEffect slash)
        {
            if (!_running) return;
            if (slash == _finalSlash)
            {
                _finalPeaked = true;
                _finalBurst.Play(slash.transform.position, slash.transform.eulerAngles.z, 1);
                HitLanded?.Invoke(1);
            }
            else
            {
                HitsLanded++;
                float progress = Mathf.Clamp01(HitsLanded / (float)_fullIntensityHits);
                _hitBurst.Play(slash.transform.position, slash.transform.eulerAngles.z, progress);
                HitLanded?.Invoke(progress);
            }
        }

        public void Stop()
        {
            _running = IsAcceptingInput = false;
            _finalSlash = null;
            _target = null;
            _slashPool?.StopAll();
            _hitBurst?.Clear();
            _finalBurst?.Clear();
        }
    }
}