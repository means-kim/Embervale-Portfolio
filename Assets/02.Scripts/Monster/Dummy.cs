using UnityEngine;

[RequireComponent(typeof(Health))]
public class Dummy : MonoBehaviour
{
    private Health _health;
    private bool _isDead;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDie += HandleDie;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDie -= HandleDie;
    }

    private void HandleDie()
    {
        if (_isDead) return;
        _isDead = true;

        // 현재 체력이 0 이하가 되어 Health가 OnDie를 발생시킨 경우.
        // 삭제 전에 EnemyDead 이펙트를 재생한다.
        if (_health != null && _health.CurrentHealth <= 0)
            EffectManager.Instance?.PlayEnemyDeath(transform.position);

        Destroy(gameObject);
    }
}