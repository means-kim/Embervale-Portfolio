using UnityEngine;
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
            for (int i = 0; i < renderers.Length; i++) renderers[i].flipX = !renderers[i].flipX;
        }
        if (_mirrorOffset)
        {
            var offset = GetComponent<VFXOffset>();
            if (offset != null) offset.MirrorX();
        }
    }
}
