using System.Collections;
using UnityEngine;

/// <summary>
/// 같은 오브젝트의 Animator/AudioSource를 자동으로 찾아 1번 재생하고 파괴한다.
/// Inspector에서 아무것도 연결할 필요 없다. EffectManager.Spawn과 함께 쓴다.
/// 역재생은 speed=-1에 의존하지 않고 시간을 직접 거꾸로 탐색한다.
/// 애니가 먼저 끝나면 스프라이트 모습만 숨기고, 사운드/파티클까지 끝나면 파괴한다.
/// </summary>
public class OneShotVFX : MonoBehaviour
{
    [Tooltip("0이면 애니/사운드/파티클 중 가장 긴 길이를 자동 사용. 지정하면 그 시간 후 파괴.")]
    [SerializeField, Min(0f)] private float _lifeTime;
    [Tooltip("가장 긴 요소가 끝난 뒤 추가로 기다릴 시간. 잔향/페이드 꼬리용.")]
    [SerializeField, Min(0f)] private float _extraDelay = 0.1f;
    [Tooltip("체크하면 애니메이션을 끝 프레임부터 거꾸로 재생한다. Death → Spawn 용.")]
    [SerializeField] private bool _playReversed;

    private Animator _animator;
    private int _stateHash;
    private float _clipLength;
    private float _reverseT;
    private bool _clipLooping;

    private void Start()
    {
        _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        float animTime = 0f;
        if (_animator != null)
        {
            float speedBefore = _animator.speed;
            _animator.Rebind();
            _animator.Update(0f); // 기본 상태 진입 확정 후 현재 클립 기준으로 재생
            AnimatorClipInfo[] clips = _animator.GetCurrentAnimatorClipInfo(0);
            if (clips != null && clips.Length > 0 && clips[0].clip != null)
            {
                _clipLength = clips[0].clip.length;
                _clipLooping = clips[0].clip.isLooping;
                float speed = Mathf.Abs(speedBefore) > 0.001f ? Mathf.Abs(speedBefore) : 1f;
                animTime = _clipLength / speed;
                _stateHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                if (_playReversed)
                {
                    // 자동 진행을 끄고 Update에서 직접 시간을 감소시킨다.
                    _animator.speed = 0f;
                    _reverseT = 0.999f;
                    _animator.Play(_stateHash, 0, _reverseT);
                    _animator.Update(0f); // 끝 프레임 즉시 반영
                }
                else
                {
                    _animator.Play(_stateHash, 0, 0f);
                }
            }
            else
            {
                _animator = null;
            }
        }

        // 사운드: 자식 포함 가장 긴 비루프 클립 기준. pitch가 바뀌면 실제 재생 길이에 맞춘다.
        float audioTime = 0f;
        var audios = GetComponentsInChildren<AudioSource>();
        foreach (var audio in audios)
        {
            if (audio == null || audio.clip == null || audio.loop)
                continue;
            float t = audio.clip.length;
            if (Mathf.Abs(audio.pitch) > 0.001f)
                t /= Mathf.Abs(audio.pitch);
            if (t > audioTime)
                audioTime = t;
            if (!audio.isPlaying)
                audio.Play();
        }

        // 파티클: 자식 포함 루프 제외하고 duration + 최대 수명 + 시작 지연 중 가장 긴 것.
        float particleTime = 0f;
        var particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            if (ps == null)
                continue;
            var main = ps.main;
            if (main.loop)
                continue;
            float t = main.duration + GetCurveMax(main.startLifetime) + GetCurveMax(main.startDelay);
            if (t > particleTime)
                particleTime = t;
        }

        float lifeTime = _lifeTime;
        if (lifeTime <= 0f)
            lifeTime = Mathf.Max(animTime, Mathf.Max(audioTime, particleTime));

        float delay = lifeTime > 0f ? lifeTime + Mathf.Max(0f, _extraDelay) : 1f;
        Destroy(gameObject, delay);

        // 애니가 파괴 시점보다 먼저 끝나면 마지막 프레임에서 멈춰 두지 않고 모습을 숨긴다.
        // 루프 클립은 끝이 없으니 제외. 파티클 렌더러는 건드리지 않아 파티클은 끝까지 보인다.
        if (_animator != null && !_clipLooping && animTime > 0f && animTime < delay)
            StartCoroutine(HideSpritesAfter(animTime));
    }

    private IEnumerator HideSpritesAfter(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sprite != null)
                sprite.enabled = false;
        }
        if (_animator != null)
            _animator.enabled = false;
    }

    private static float GetCurveMax(ParticleSystem.MinMaxCurve curve)
    {
        switch (curve.mode)
        {
            case ParticleSystemCurveMode.Constant:
                return Mathf.Max(0f, curve.constant);
            case ParticleSystemCurveMode.TwoConstants:
                return Mathf.Max(0f, curve.constantMax);
            default:
                // 커브 모드에서는 constantMax에 최대값이 들어간다. 비어 있으면 0.
                return Mathf.Max(0f, curve.constantMax);
        }
    }

    private void Update()
    {
        if (!_playReversed || _animator == null || _clipLength <= 0f) return;
        _reverseT -= Time.deltaTime / _clipLength;
        if (_reverseT < 0f) _reverseT = 0f;
        _animator.Play(_stateHash, 0, _reverseT);
        _animator.Update(0f);
    }

    /// <summary>
    /// 즉시 파괴한다. 보통은 마지막 프레임 이벤트에 걸 필요가 없다.
    /// 애니 종료 시 스프라이트가 자동으로 숨고 사운드/파티클이 끝나면 파괴되기 때문이다.
    /// </summary>
    public void StopEffect()
    {
        Destroy(gameObject);
    }
}
