using UnityEngine;

public class TelegraphPool : MonoBehaviour
{
    private static TelegraphPool _instance;
    public static TelegraphPool Instance => _instance;

    [Header("텔레 그래프 프리팹들")]
    [SerializeField] private Telegraph[] _telegraphPrefabs;

    [Header("풀 사이즈")]
    [SerializeField] private int _poolSize = 5;

    private Telegraph[,] _pool;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _pool = new Telegraph[_telegraphPrefabs.Length, _poolSize];

        for (int i = 0; i < _telegraphPrefabs.Length; i++)
        {
            Telegraph telegraphPrefab = _telegraphPrefabs[i];

            for (int j = 0; j < _poolSize; j++)
            {
                Telegraph telegraph = Instantiate(telegraphPrefab, gameObject.transform);
                telegraph.gameObject.SetActive(false);
                _pool[i, j] = telegraph;
            }
        }
    }

    private Telegraph GetTelegraph(TelegraphType telegraphType)
    {
        for (int i = 0; i < _telegraphPrefabs.Length; i++)
        {
            if (_pool[i, 0].Type != telegraphType) continue;

            for (int j = 0; j < _poolSize; j++)
            {
                Telegraph telegraph = _pool[i, j];

                if (telegraph.TryReserve())
                {
                    return telegraph;
                }
            }
        }

        return null;
    }
}