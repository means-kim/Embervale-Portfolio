using UnityEngine;
[CreateAssetMenu(fileName = "NewSFXData", menuName = "Audio/SFX Data")]
public class SFXData : ScriptableObject
{
    public AudioClip Clip;
    [Range(0f, 1f)] public float Volume = 1f;
    [Range(0.5f, 1.5f)] public float Pitch = 1f;
    public void Play()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(this);
    }
}