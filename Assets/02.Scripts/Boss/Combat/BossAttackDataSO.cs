using UnityEngine;

[CreateAssetMenu(fileName = "BossAttackDataSO", menuName = "Scriptable Objects/BossAttackDataSO")]
public class BossAttackDataSO : ScriptableObject
{
    [SerializeField] private BossAttackData[] _attacks;

    public BossAttackData[] Attacks => _attacks;
}