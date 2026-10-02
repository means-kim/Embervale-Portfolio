using UnityEngine;
[CreateAssetMenu(menuName = "Player/Attack Data", fileName = "PlayerAttack_")]
public class PlayerAttackDataSO : ScriptableObject
{
    [Header("발동")]
    public PlayerAttackType attackType;
    public KeyCode triggerKey = KeyCode.Alpha1;
    [Tooltip("보조 키. None이면 쓰지 않는다.")]
    public KeyCode altTriggerKey = KeyCode.None;
    [Header("판정·스폰 (Offset 하나로 통일)")]
    public PlayerAttackHitbox hitbox = new()
    {
        Size = new Vector2(1.4f, 1.2f),
        Offset = new Vector2(0.8f, 0.8f),
        Duration = 0.5f,
    };
    [Header("연출")]
    [Tooltip("재생할 플레이어 애니 클립명. 비어 있으면 LeftHorizontalAttack.")]
    public string animClipName = "LeftHorizontalAttack";
    [Tooltip("타격 지점 VFX 프리팹. 비어 있으면 생략.")]
    public GameObject vfxPrefab;
    [Header("투사체 (지정 시 데미지는 투사체가 전담)")]
    [Tooltip("IPlayerProjectile 포함 프리팹. 비어 있으면 근접 히트박스 판정.")]
    public GameObject projectilePrefab;
    [Header("데미지")]
    [Min(0)] public int damage = 10;
}
