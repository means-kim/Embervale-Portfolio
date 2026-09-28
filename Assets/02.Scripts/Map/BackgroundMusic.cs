using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public enum BgmType
    {
        Snow,
        Cave,
        IntoBoss,
        Boss
    }

    private static BackgroundMusic _instance;
    public static BackgroundMusic instance => _instance;

    [Header("음악 클립")]
    [SerializeField] AudioClip _snowBgm;
    [SerializeField] AudioClip _caveBgm;
    [SerializeField] AudioClip _intoBossBgm;
    [SerializeField] AudioClip _bossBgm;

    [Header("재생 소스")]
    [SerializeField] private AudioSource _mainSource;
    [SerializeField] private AudioSource _layerSource;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    private AudioSource GetSource(bool layer)
    {
        return layer ? _layerSource : _mainSource;
    }
    public static void PlayBgm(BgmType type, bool layer = false, float volume = 1f)
    {
        if (_instance == null)
        {
            return;
        }

        AudioSource source = _instance.GetSource(layer);
        AudioClip clip;

        switch (type)
        {
            case BgmType.Cave:
                {
                    clip = _instance._caveBgm;
                    break;
                }
            case BgmType.IntoBoss:
                {
                    clip = _instance._intoBossBgm;
                    break;
                }
            case BgmType.Boss:
                {
                    clip = _instance._bossBgm;
                    break;
                }
            default:
                {
                    clip = _instance._snowBgm;
                    break;
                }
        }

        if (source == null || clip == null)
        {
            return;
        }

        source.volume = Mathf.Clamp01(volume);
        if (source.clip == clip && source.isPlaying)
        {
            return;
        }

        source.Stop();
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    public static void SetVolume(float volume, bool layer = false)
    {
        if (_instance == null)
        {
            return;
        }

        AudioSource source = _instance.GetSource(layer);

        if (source != null)
        {
            source.volume = Mathf.Clamp01(volume);
        }
    }
    public static void StopBgm(bool layer = false)
    {
        if (_instance == null)
        {
            return;
        }

        AudioSource source = _instance.GetSource(layer);

        if (source != null)
        {
            source.Stop();
        }
    }

}
