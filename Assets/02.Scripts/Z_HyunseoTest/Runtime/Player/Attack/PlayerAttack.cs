using System;
using System.Collections.Generic;
using UnityEngine;

namespace HyunseoTest
{
    [RequireComponent(typeof(PlayerMove))]
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _comboResetDelay = 0.6f;
        [SerializeField, Min(0f)] private float _comboInputBuffer = 0.2f;
        private PlayerAttackDataSO[] _combo;
        private int _nextCombo;
        private float _comboExpires = -1f;
        private bool _queuedCombo;
        private bool _isComboAttack;
        private readonly Dictionary<PlayerAttackDataSO, float> _nextAttackTimes = new();
        public void RequestCombo(PlayerAttackDataSO first, PlayerAttackDataSO second)
        {
            _combo = new[] { first, second };
            if (IsAttacking)
            {
                float remaining = Mathf.Max(0, _current.windupDuration) + Mathf.Max(0.05f, _current.recoveryDuration) - _elapsed;
                if (_isComboAttack && remaining <= _comboInputBuffer) _queuedCombo = true;
                return;
            }
            StartNextCombo();
        }
        private void StartNextCombo()
        {
            if (_combo == null) return;
            int index = Time.time > _comboExpires ? 0 : _nextCombo;
            if (!TryStartAttack(_combo[index])) return;
            _isComboAttack = true;
            _nextCombo = (index + 1) % _combo.Length;
        }
        private PlayerMove _move;
        private PlayerAttackDataSO _current;
        private PlayerAttackInstance _instance;
        private float _elapsed;
        private int _direction;
        private bool _spawned;
        public event Action OnAttackStarted;
        public event Action OnAttackEnded;
        public bool IsAttacking { get; private set; }
        public string CurrentAttackClip => _current != null ? _current.animClipName : "LeftHorizontalAttack";
        private void Awake() => _move = GetComponent<PlayerMove>();
        private void OnDisable() => CancelAttack();

        // 공격 허용 여부는 PlayerController가 결정한다. 입력이나 프리팹 종류를 판별하지 않는다.
        public bool TryStartAttack(PlayerAttackDataSO definition)
        {
            var controller = GetComponent<PlayerController>();
            if (!isActiveAndEnabled || IsAttacking || definition == null || controller == null || !controller.CanAttack)
            {
                return false;
            }
            if (definition.attackPrefab == null || string.IsNullOrWhiteSpace(definition.animClipName))
            {
                Debug.LogError($"공격 프리팹 또는 애니메이션이 비어 있습니다: {definition.name}", definition);
                return false;
            }
            if (_nextAttackTimes.TryGetValue(definition, out float readyAt) && Time.time < readyAt) return false;
            _current = definition;
            _elapsed = 0;
            _spawned = false;
            _direction = _move.FacingDirection;
            IsAttacking = true;
            _isComboAttack = false; // Attacks do not own a movement lock.
            OnAttackStarted?.Invoke();
            if (IsAttacking && definition.windupDuration <= 0) SpawnAttack();
            return true;
        }
        private void Update()
        {
            if (!IsAttacking) return;
            _elapsed += Time.deltaTime;
            if (!_spawned && _elapsed >= Mathf.Max(0, _current.windupDuration)) SpawnAttack();
            if (IsAttacking && _elapsed >= Mathf.Max(0, _current.windupDuration) + Mathf.Max(0.05f, _current.recoveryDuration))
                FinishAttack(true);
        }
        private void SpawnAttack()
        {
            _spawned = true;
            Vector3 position = transform.position + new Vector3(_direction * _current.spawnOffset.x, _current.spawnOffset.y);
            _instance = Instantiate(_current.attackPrefab, position, Quaternion.identity);
            if (!_instance.Initialize(_current.damage, _direction, transform))
            {
                Destroy(_instance.gameObject);
                _instance = null;
                FinishAttack();
            }
        }
        public void CancelAttack() => FinishAttack();
        // Called by events on the copied player clips, not by input or a free-running timer.
        public void OpenAttackHitbox(string clipName)
        {
            if (IsAttacking && _current != null && _current.animClipName == clipName && _instance != null)
                _instance.OpenAnimationWindow();
        }
        public void CloseAttackHitbox(string clipName)
        {
            if (IsAttacking && _current != null && _current.animClipName == clipName && _instance != null)
                _instance.StopDamage();
        }
        private void FinishAttack(bool completed = false)
        {
            if (!IsAttacking) return;
            bool continueCombo = completed && _isComboAttack && _queuedCombo;
            _queuedCombo = false;
            _comboExpires = completed && _isComboAttack ? Time.time + _comboResetDelay : -1f;
            // Also apply on cancellation so interrupting the attack cannot bypass its repeat delay.
            if (_current != null && _current.repeatDelay > 0f)
                _nextAttackTimes[_current] = Time.time + _current.repeatDelay;
            IsAttacking = false;
            if (_instance != null) _instance.EndPlayerAction();
            _instance = null;
            // Other movement locks (wall jump, hit, dash) are not modified.
            OnAttackEnded?.Invoke();
            if (continueCombo) StartNextCombo();
        }
    }

}
