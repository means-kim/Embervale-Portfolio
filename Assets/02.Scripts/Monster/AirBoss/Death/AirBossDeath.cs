using UnityEngine;

public class AirBossDeath : MonoBehaviour
{
    [SerializeField] private Health _airBossossHealth;
    [SerializeField] private AirBossController _airBossController;
    [SerializeField] private AirBossAnimation _airBossAnimation;
    [SerializeField] private CircleCollider2D _airBossCollider;
    [SerializeField] private Rigidbody2D _airBossRB;

    private bool _isDead = false;

    private void OnEnable()
    {
        _airBossossHealth.OnDie += HandleDeath;
    }

    private void OnDisable()
    {
        _airBossossHealth.OnDie -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (_isDead) return;
        _isDead = true;

        // Death 상태로 전환하고 이동과 공격 제한
        _airBossController.EnterDeath();

        // 이미 발사된 공격체 반환
        AirBossAttackPool.Instance?.ReleaseAll();

        _airBossCollider.enabled = false;
        _airBossRB.linearVelocity = Vector2.zero;
        _airBossRB.angularVelocity = 0f;
        _airBossRB.simulated = false;

        // The ending owns the final blow. Without an enabled matching ending, keep normal death.
        var ending = FindFirstObjectByType<GameEndingCinematic>();
        if (ending == null || !ending.HandlesBossDeath(_airBossAnimation))
            _airBossAnimation.PlayDeathAnimation();
    }
}
