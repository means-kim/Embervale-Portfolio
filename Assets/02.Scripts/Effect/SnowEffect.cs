using UnityEngine;
public class SnowEffect : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ParticleSystem _particle;
    private bool _animationFinish;
    private SnowEffectType _type;
    private SnowEffectPool _pool;
    private bool _isPlaying;
    private Vector3 _defaultScale;
    private void Awake() => _defaultScale = transform.localScale;
    public void Play(SnowEffectType type, SnowEffectPool pool, Vector3? scale = null, bool flipX = false)
    {
        _type = type;
        _pool = pool;
        transform.localScale = scale ?? _defaultScale;
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
            _animationFinish = false;
        }
        else
        {
            _animationFinish = true;
        }
        if (_spriteRenderer != null) _spriteRenderer.flipX = type == SnowEffectType.Explode && flipX;
        if (_particle != null)
        {
            _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particle.Play(true);
        }
        _isPlaying = true;
    }
    private void Update()
    {
        if (!_isPlaying) return;
        bool particlesPlaying = _particle != null && _particle.IsAlive(true);
        if (!_animationFinish || particlesPlaying) return;
        _isPlaying = false;
        _pool.ReleaseEffect(_type, gameObject);
    }
    public void OnAnimationFinished() => _animationFinish = true;
    private void OnDisable()
    {
        _isPlaying = false;
        if (_particle != null) _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}