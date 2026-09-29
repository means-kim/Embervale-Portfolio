using UnityEngine;

[CreateAssetMenu(fileName = "EliteMonsterAttackDataSO", menuName = "Scriptable Objects/EliteMonsterAttackDataSO")]
public class EliteMonsterAttackDataSO : ScriptableObject
{
    [SerializeField] private EliteMonsterAttackData[] _attacks;

    public EliteMonsterAttackData[] Attacks => _attacks;
}