using UnityEngine;

[System.Serializable]
public class EliteMonsterAttackData
{
    [SerializeField] private EliteMonsterAttackType _type;
    [SerializeField] private int _weight;

    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;

    [SerializeField] private int _damage;

    [SerializeField] private float _recoveryTime;
    [SerializeField] private float _coolTime;

    public EliteMonsterAttackType Type => _type;
    public int Weight => _weight;

    public float MinDistance => _minDistance;
    public float MaxDistance => _maxDistance;

    public int Damage => _damage;
    public float RecoveryTime => _recoveryTime;
    public float CoolTime => _coolTime;
}