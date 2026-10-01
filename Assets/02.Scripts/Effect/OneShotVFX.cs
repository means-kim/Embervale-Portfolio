using UnityEngine;

/// <summary>
/// 같은 오브젝트의 Animator/AudioSource를 자동으로 찾아 1번 재생하고 파괴한다.
/// Inspector에서 아무것도 연결할 필요 없다. EffectManager.Spawn과 함께 쓴다.
/// 역재생은 speed=-1에 의존하지 않고 시간을 직접 거꾸로 탐색한다.
/// </summary>
public class OneShotVFX : MonoBehaviour
{
    [Tooltip("0이면 클립 길이를 자동 사용. 지정하면 그 시간 후 파괴.")]
    [SerializeField, Min(0f)] private float _lifeTime;
    [Tooltip("체크하면 애니메이션을 끝 프레임부터 거꾸로 재생한다. Death → Spawn 용.")]
    [SerializeField] private bool _playReversed;

    private Animator _animator;
    private int _stateHash;
    private float _clipLength;
    private float _reverseT;

    private void Start()
    {
        _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        float lifeTime = _lifeTime;
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f); // 기본 상태 진입 확정 후 현재 클립 기준으로 재생
            AnimatorClipInfo[] clips = _animator.GetCurrentAnimatorClipInfo(0);
            if (clips != null && clips.Length > 0 && clips[0].clip != null)
            {
                _clipLength = clips[0].clip.length;
                if (lifeTime <= 0f) lifeTime = _clipLength;
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

        var audio = GetComponent<AudioSource>() ?? GetComponentInChildren<AudioSource>();
        if (audio != null && audio.clip != null && !audio.isPlaying)
            audio.Play();
        if (lifeTime <= 0f && audio != null && audio.clip != null)
            lifeTime = audio.clip.length;

        Destroy(gameObject, lifeTime > 0f ? lifeTime : 1f);
    }

    private void Update()
    {
        if (!_playReversed || _animator == null || _clipLength <= 0f) return;
        _reverseT -= Time.deltaTime / _clipLength;
        if (_reverseT < 0f) _reverseT = 0f;
        _animator.Play(_stateHash, 0, _reverseT);
        _animator.Update(0f);
    }

    /// <summary>애니메이션 마지막 프레임 이벤트에서 호출해도 된다. 중복 호출 안전.</summary>
    public void StopEffect()
    {
        Destroy(gameObject);
    }
}
