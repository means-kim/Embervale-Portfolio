using UnityEngine;

/// <summary>
/// 공격 1종 정의. PlayerAttack의 목록에 연결하면 발동 키·판정·연출이 그대로 동작한다.
/// 콤보는 이런 SO를 순서대로 나열하는 방식으로 확장한다.
/// </summary>
[CreateAssetMenu(menuName = "Player/Attack Data", fileName = "PlayerAttack_")]
public class PlayerAttackDataSO : ScriptableObject
{
    [Header("발동")]
    public PlayerAttackType attackType;
    public KeyCode triggerKey = KeyCode.Alpha1;
    [Tooltip("보조 키. None이면 쓰지 않는다.")]
    public KeyCode altTriggerKey = KeyCode.None;

    [Header("판정 (투사체 지정 시 Size/Offset은 쓰지 않음)")]
    public PlayerAttackHitbox hitbox = new()
    {
        Size = new Vector2(1.4f, 1.2f),
        Offset = new Vector2(0.8f, 0.8f),
        VfxOffset = new Vector2(0.8f, 0.8f),
        Duration = 0.5f,
    };

    [Header("연출")]
    [Tooltip("재생할 플레이어 애니 클립명. 비어 있으면 SideAttack.")]
    public string animClipName = "SideAttack";
    [Tooltip("타격 지점 VFX 프리팹. 비어 있으면 생략.")]
    public GameObject vfxPrefab;

    [Header("투사체 (지정 시 데미지는 투사체가 전담)")]
    [Tooltip("StabProjectile 포함 프리팹. 비어 있으면 즉발 박스 판정.")]
    public GameObject projectilePrefab;

    [Header("데미지")]
    [Min(0)] public int damage = 10;
}
