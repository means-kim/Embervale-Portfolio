using UnityEngine;

[CreateAssetMenu(menuName = "Player/Attack Data", fileName = "PlayerAttack_")]
public class PlayerAttackDataSO : ScriptableObject
{
    public PlayerAttackType attackType;
    public PlayerAttackInstance attackPrefab;
    [Tooltip("플레이어 기준 월드 단위 스폰 위치. x만 바라보는 방향에 맞춰 반전한다.")]
    public Vector2 spawnOffset;
    public string animClipName = "LeftHorizontalAttack";
    [Min(0)] public int damage = 10;
    [Tooltip("공격 애니메이션 시작 후 공격 객체를 생성할 때까지의 시간.")]
    [Min(0)] public float windupDuration;
    [Tooltip("생성 후 다음 행동을 허용하기까지의 시간. 판정 수명은 프리팹에서 설정한다.")]
    [Min(0.05f)] public float recoveryDuration = 0.22f;
}
