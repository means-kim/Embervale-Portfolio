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

        // ToDo: 사망 애니메이션 1프레임에 사망 컷신 연출 이벤트 함수 호출
        _airBossAnimation.PlayDeathAnimation();
    }
}