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

        _airBossCollider.enabled = false;
        _airBossRB.velocity = Vector2.zero;
        _airBossRB.angularVelocity = 0f;
        _airBossRB.simulated = false;

        // ToDo: 사망 애니메이션 1프레임에 사망 컷신 연출 이벤트 함수 호출
        _airBossAnimation.PlayDeathAnimation();
    }
}