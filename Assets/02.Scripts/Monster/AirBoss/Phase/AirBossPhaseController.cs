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
}