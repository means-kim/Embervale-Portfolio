using UnityEngine;
[CreateAssetMenu(menuName = "Player/Attack Data", fileName = "PlayerAttack_")]
public class PlayerAttackDataSO : ScriptableObject
{
    public PlayerAttackType AttackType;
    public PlayerAttackInstance AttackPrefab;
    [Tooltip("플레이어 기준 월드 단위 스폰 위치")]
    public Vector2 SpawnOffset;
    public string AnimClipName = "FirstAttack";
    [Min(0)] public int Damage = 10;
    [Min(0), Tooltip("공격 발동 시 소모하는 플레이어 체력")]
    public int HealthCost;
    [Tooltip("공격 애니메이션 시작 후 공격 객체를 생성할 때까지의 시간")]
    [Min(0)] public float WindupDuration;
    [Tooltip("생성 후 다음 행동을 허용하기까지의 시간")]
    [Min(0.05f)] public float RecoveryDuration = 0.22f;
}
