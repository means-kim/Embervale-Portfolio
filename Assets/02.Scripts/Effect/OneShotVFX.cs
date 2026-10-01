using UnityEngine;

/// <summary>
/// 같은 오브젝트의 Animator/AudioSource를 자동으로 찾아 1번 재생하고 파괴한다.
/// Inspector에서 아무것도 연결할 필요 없다. EffectManager.Spawn과 함께 쓴다.
/// </summary>
public class OneShotVFX : MonoBehaviour
{
    [Tooltip("0이면 클립 길이를 자동 사용. 지정하면 그 시간 후 파괴.")]
    [SerializeField, Min(0f)] private float _lifeTime;

    private void Awake()
    {
        var animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.Play(0, 0, 0f);
        }

        var audio = GetComponent<AudioSource>() ?? GetComponentInChildren<AudioSource>();
        if (audio != null && audio.clip != null && !audio.isPlaying)
            audio.Play();

        Destroy(gameObject, ResolveLifeTime(animator, audio));
    }

    private float ResolveLifeTime(Animator animator, AudioSource audio)
    {
        if (_lifeTime > 0f) return _lifeTime;
        if (animator != null)
        {
            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips != null && clips.Length > 0 && clips[0].clip != null)
                return clips[0].clip.length;
        }
        if (audio != null && audio.clip != null)
            return audio.clip.length;
        return 1f;
    }

    /// <summary>애니메이션 마지막 프레임 이벤트에서 호출해도 된다. 중복 호출 안전.</summary>
    public void StopEffect()
    {
        Destroy(gameObject);
    }
}
