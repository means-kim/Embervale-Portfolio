using UnityEngine;

/// <summary>
/// VFX 프리팹에 붙이면 생성 위치에서 추가로 nudge한다.
/// 호출 쪽(PlayerAttack 등)이 준 base 위치는 유지하고, 프리팹별 스프라이트 피벗 보정만 담당.
/// Inspector 값만 조절, 연결 불필요.
/// </summary>
public class VFXOffset : MonoBehaviour
{
    [Tooltip("생성 위치에 더할 오프셋. 좌우 반전이 필요하면 호출 쪽에서 처리한다.")]
    [SerializeField] private Vector3 _offset;

    private void Start()
    {
        transform.position += _offset;
    }
}
