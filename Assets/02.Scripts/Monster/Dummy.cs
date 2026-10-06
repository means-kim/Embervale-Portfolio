using UnityEngine;
[RequireComponent(typeof(Health))]
public class Dummy : MonoBehaviour
{
    private Health _health;
    private bool _isDead;
    private void Awake() => _health = GetComponent<Health>();
    private void OnEnable() => _health.OnDie += HandleDie;
    private void OnDisable() => _health.OnDie -= HandleDie;
    private void HandleDie()
    {
        if (_isDead) return;
        _isDead = true;
        if (_health.CurrentHealth <= 0) EffectManager.Instance?.PlayBossDeath(transform.position);
        Destroy(gameObject);
    }
}
