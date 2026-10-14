using System.Collections;
using UnityEngine;
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
            _animator.Update(0f);
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
                    _animator.speed = 0f;
                    _reverseT = 0.999f;
                    _animator.Play(_stateHash, 0, _reverseT);
                    _animator.Update(0f);
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
        float audioTime = 0f;
        var audios = GetComponentsInChildren<AudioSource>();
        foreach (var audio in audios)
        {
            SettingsAudioSource.Bind(audio);
            if (audio == null || audio.clip == null || audio.loop) continue;
            float t = audio.clip.length;
            if (Mathf.Abs(audio.pitch) > 0.001f) t /= Mathf.Abs(audio.pitch);
            if (t > audioTime) audioTime = t;
            if (!audio.isPlaying) audio.Play();
        }
        float particleTime = 0f;
        var particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            if (ps == null) continue;
            var main = ps.main;
            if (main.loop) continue;
            float t = main.duration + GetCurveMax(main.startLifetime) + GetCurveMax(main.startDelay);
            if (t > particleTime) particleTime = t;
        }
        float lifeTime = _lifeTime;
        if (lifeTime <= 0f) lifeTime = Mathf.Max(animTime, Mathf.Max(audioTime, particleTime));
        float delay = lifeTime > 0f ? lifeTime + Mathf.Max(0f, _extraDelay) : 1f;
        Destroy(gameObject, delay);
        if (_animator != null && !_clipLooping && animTime > 0f && animTime < delay) StartCoroutine(HideSpritesAfter(animTime));
    }
    private IEnumerator HideSpritesAfter(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sprite != null) sprite.enabled = false;
        }
        if (_animator != null) _animator.enabled = false;
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
    public void StopEffect() => Destroy(gameObject);
}
