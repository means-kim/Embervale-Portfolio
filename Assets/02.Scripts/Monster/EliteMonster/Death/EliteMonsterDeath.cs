using UnityEngine;
using System.Collections;

public class EliteMonsterDeath : MonoBehaviour
{
    [SerializeField] private EliteMonsterMovement _eliteMonsterMovement;
    [SerializeField] private Health _bossHealth;
    [SerializeField] private EliteMonsterFacing _eliteMonsterFacing;
    [SerializeField] private Animator _animator;
    [SerializeField] private EliteMonsterAttackController _eliteMonsterAttackController;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private CapsuleCollider2D _bodyCollider;

    [SerializeField] private float _deathAnimationTime = 1.4f;
    [SerializeField] private float _fadeDuration = 1.0f;

    private SpriteRenderer _spriteRenderer;
    private bool _deathStarted;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _bossHealth.OnDie += HandleDeath;
    }

    private void OnDisable()
    {
        _bossHealth.OnDie -= HandleDeath;
    }

    private void HandleDeath()
    {
        EffectManager.Instance?.PlayBossDeath(transform.position);

        _eliteMonsterAttackController.CancelCurrentAttack();

        _eliteMonsterMovement.enabled = false;
        _eliteMonsterFacing.enabled = false;

        DisableBodyCollision();

        _animator.SetBool("IsWalk", false);
        _animator.SetTrigger("IsDeath");
    }

    private void DisableBodyCollision()
    {
        if (!_bossHealth.IsDead) return;

        _rigidBody.simulated = false;
        _bodyCollider.enabled = false;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // 사망 애니메이션이 재생될 시간을 기다립니다.
        yield return new WaitForSeconds(_deathAnimationTime);

        float startAlpha = _spriteRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;

            Color color = _spriteRenderer.color;
            color.a = Mathf.Lerp(
                startAlpha,
                0f,
                Mathf.Clamp01(elapsed / _fadeDuration)
            );

            _spriteRenderer.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }
}