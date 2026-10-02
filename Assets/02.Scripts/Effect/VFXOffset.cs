using UnityEngine;
public class VFXOffset : MonoBehaviour
{
    [Tooltip("생성 위치에 더할 오프셋. 좌우 반전이 필요하면 호출 쪽에서 처리한다.")]
    [SerializeField] private Vector3 _offset;
    public void MirrorX() => _offset.x = -_offset.x;
    private void Start() => transform.position += _offset;
}
