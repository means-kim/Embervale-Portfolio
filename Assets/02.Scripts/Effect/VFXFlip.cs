using UnityEngine;

/// <summary>
/// VFX 프리팹 루트에 붙이면 EffectManager.Spawn(prefab, pos, direction)이
/// direction이 왼쪽(-)일 때 이 프리팹을 좌우 반전한다.
/// Inspector 값만 조절, 연결 불필요. 아트는 오른쪽 기준.
/// 파티클의 운동 방향은 뒤집지 못하므로 스프라이트 VFX 전용.
/// </summary>
public class VFXFlip : MonoBehaviour
{
    [Tooltip("체크하면 왼쪽 스폰 시 자식 포함 전 SpriteRenderer.flipX를 토글한다.")]
    [SerializeField] private bool _flipSprites = true;
    [Tooltip("체크하면 같은 오브젝트의 VFXOffset.x도 함께 미러링한다.")]
    [SerializeField] private bool _mirrorOffset = true;

    public void Apply(int direction)
    {
        if (direction >= 0) return;
        if (_flipSprites)
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].flipX = !renderers[i].flipX;
        }
        if (_mirrorOffset)
        {
            var offset = GetComponent<VFXOffset>();
            if (offset != null) offset.MirrorX();
        }
    }
}
