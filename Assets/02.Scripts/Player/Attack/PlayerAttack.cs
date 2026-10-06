using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMove))]
public class PlayerAttack : MonoBehaviour
{
    private PlayerMove _move;
    private PlayerAttackDataSO _current;
    private PlayerAttackInstance _instance;
    private float _elapsed;
    private int _direction;
    private bool _spawned;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    public string CurrentAttackClip => _current != null ? _current.AnimClipName : "LeftHorizontalAttack";
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
        if (definition.AttackPrefab == null || string.IsNullOrWhiteSpace(definition.AnimClipName))
        {
            Debug.LogError($"공격 프리팹 또는 애니메이션이 비어 있습니다: {definition.name}", definition);
            return false;
        }
        _current = definition;
        _elapsed = 0;
        _spawned = false;
        _direction = _move.FacingDirection;
        IsAttacking = true;
        _move.SetMovementLocked(this, true);
        OnAttackStarted?.Invoke();
        if (IsAttacking && definition.WindupDuration <= 0) SpawnAttack();
        return true;
    }
    private void Update()
    {
        if (!IsAttacking) return;
        _elapsed += Time.deltaTime;
        if (!_spawned && _elapsed >= Mathf.Max(0, _current.WindupDuration)) SpawnAttack();
        if (IsAttacking && _elapsed >= Mathf.Max(0, _current.WindupDuration) + Mathf.Max(0.05f, _current.RecoveryDuration)) FinishAttack();
    }
    private void SpawnAttack()
    {
        _spawned = true;
        Vector3 position = transform.position + new Vector3(_direction * _current.SpawnOffset.x, _current.SpawnOffset.y);
        _instance = Instantiate(_current.AttackPrefab, position, Quaternion.identity);
        if (!_instance.Initialize(_current.Damage, _direction, transform))
        {
            Destroy(_instance.gameObject);
            _instance = null;
            FinishAttack();
        }
    }
    public void CancelAttack() => FinishAttack();
    private void FinishAttack()
    {
        if (!IsAttacking) return;
        IsAttacking = false;
        if (_instance != null) _instance.EndPlayerAction();
        _instance = null;
        if (_move != null) _move.SetMovementLocked(this, false);
        OnAttackEnded?.Invoke();
    }
}
