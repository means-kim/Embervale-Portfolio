using UnityEngine;
public class CameraShakeOnce : MonoBehaviour
{
    [Tooltip("흔들림 강도. CameraManager.ShakeCamera로 전달된다.")]
    [SerializeField, Min(0f)] private float _strength = 1f;
    private void Start() => CameraManager.ShakeCamera(_strength);
}