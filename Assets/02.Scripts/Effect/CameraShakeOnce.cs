using UnityEngine;

/// <summary>
/// VFX 프리팹에 붙이면 생성 시 카메라를 한 번 흔든다.
/// Inspector 연결 없이 강도 값만 조절한다. 실제 흔들기는 CameraManager에 위임.
/// </summary>
public class CameraShakeOnce : MonoBehaviour
{
    [Tooltip("흔들림 강도. CameraManager.ShakeCamera로 전달된다.")]
    [SerializeField, Min(0f)] private float _strength = 1f;

    private void Start()
    {
        CameraManager.ShakeCamera(_strength);
    }
}
