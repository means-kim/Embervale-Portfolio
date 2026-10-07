using UnityEngine;

public class AirBossAttackPool : MonoBehaviour
{
    private static AirBossAttackPool _instance;
    public static AirBossAttackPool Instance => _instance;

    [Header("공격 프리팹들")]
    [SerializeField] private AirBossAttack[] _attackPrefabs;

    [Header("풀 사이즈")]
    [SerializeField] private int _poolSize = 10;

    private AirBossAttack[,] _pool;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _pool = new AirBossAttack[_attackPrefabs.Length, _poolSize];

        for (int i = 0; i < _attackPrefabs.Length; i++)
        {
            AirBossAttack attackPrefab = _attackPrefabs[i];

            for (int j = 0; j < _poolSize; j++)
            {
                AirBossAttack attack = Instantiate(attackPrefab, gameObject.transform);
                attack.gameObject.SetActive(false);
                _pool[i, j] = attack;
            }
        }
    }

    public AirBossAttack GetAttack(AirBossAttackType attackType)
    {
        for (int i = 0; i < _attackPrefabs.Length; i++)
        {
            if (_pool[i, 0].Type != attackType) continue;

            for (int j = 0; j < _poolSize; j++)
            {
                AirBossAttack attack = _pool[i, j];

                if (attack.TryReserve())
                {
                    return attack;
                }
            }
        }

        return null;
    }

    public void ReleaseAll()
    {
        if (_pool == null) return;

        foreach (AirBossAttack attack in _pool)
        {
            if (attack != null)
            {
                attack.Release();
            }
        }
    }
}