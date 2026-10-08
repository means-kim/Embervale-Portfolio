using UnityEngine;
using System;

public class AirBossPhaseController : MonoBehaviour
{
    [SerializeField] private Health _airBossHealth;

    [Header("페이즈2 체력 퍼센트")]
    [SerializeField] private float _phase2HealthPercent = 70f;

    [Header("페이즈3 체력 퍼센트")]
    [SerializeField] private float _phase3HealthPercent = 30f;

    private int _currentPhase = 1;
    private int _pendingPhase;
    private bool _isTransition = false;

    public int CurrentPhase => _currentPhase;
    public int PendingPhase => _pendingPhase;
    public bool IsTransition => _isTransition;

    public event Action<int> TransitionRequested;
    public event Action<int> PhaseChange;

    private void OnEnable()
    {
        if (_airBossHealth == null) return;

        _airBossHealth.OnHit += CheckPhase;
        _airBossHealth.OnDie += CancelTransition;
    }

    private void OnDisable()
    {
        if (_airBossHealth != null)
        {
            _airBossHealth.OnHit -= CheckPhase;
            _airBossHealth.OnDie -= CancelTransition;
        }

        CancelTransition();
    }

    public void CheckPhase()
    {
        if (_airBossHealth == null || _airBossHealth.IsDead) return;
        if (_airBossHealth.MaxHealth <= 0) return;
        if (_isTransition || _currentPhase >= 3) return;

        float healthPercent = (float)_airBossHealth.CurrentHealth / _airBossHealth.MaxHealth * 100f;

        float phasePercent = _currentPhase == 1 ? _phase2HealthPercent : _phase3HealthPercent;

        if (healthPercent > phasePercent) return;

        _pendingPhase = _currentPhase + 1;
        _isTransition = true;

        TransitionRequested?.Invoke(_pendingPhase);
    }

    public void CompletePhase()
    {
        if (!_isTransition) return;

        if (_airBossHealth == null || _airBossHealth.IsDead)
        {
            CancelTransition();
            return;
        }

        _currentPhase = _pendingPhase;
        _pendingPhase = 0;
        _isTransition = false;

        PhaseChange?.Invoke(_currentPhase);
    }

    public void CancelTransition()
    {
        _pendingPhase = 0;
        _isTransition = false;
    }
}