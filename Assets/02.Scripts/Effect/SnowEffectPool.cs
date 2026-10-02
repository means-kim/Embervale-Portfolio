using UnityEngine;
using UnityEngine.Pool;
public class SnowEffectPool : MonoBehaviour
{
    private static SnowEffectPool _instance = null;
    public static SnowEffectPool Instance => _instance;
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private GameObject _powderPrefab;
    [SerializeField] private GameObject _explodePrefab;
    [SerializeField] private int _poolSize = 10;
    private ObjectPool<GameObject> _particlePool;
    private ObjectPool<GameObject> _powderPool;
    private ObjectPool<GameObject> _explodePool;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        _particlePool = CreatePool(_particlePrefab);
        _powderPool = CreatePool(_powderPrefab);
        _explodePool = CreatePool(_explodePrefab);
    }
    private ObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => CreateEffect(prefab),
            TakeFromPool,
            ReturnToPool,
            DestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: 5,
            maxSize: _poolSize
        );
    }

    private GameObject CreateEffect(GameObject prefab)
    {
        GameObject effect = Instantiate(prefab, transform);
        effect.SetActive(false);
        return effect;
    }
    private void TakeFromPool(GameObject obj)
    {
        obj.SetActive(true);
    }
    private void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
    private void DestroyPoolObject(GameObject obj)
    {
        Destroy(obj);
    }
    private ObjectPool<GameObject> GetPool(SnowEffectType type)
    {
        switch (type)
        {
            case SnowEffectType.Particle:
                return _particlePool;

            case SnowEffectType.Powder:
                return _powderPool;

            case SnowEffectType.Explode:
                return _explodePool;
            default: return null;
        }
    }

    public GameObject GetEffect(SnowEffectType type)
    {
        return GetPool(type).Get();
    }
    public void ReleaseEffect(SnowEffectType type, GameObject effect)
    {
        GetPool(type).Release(effect);
    }
    private void OnDestroy()
    {
        if (_instance != this) return;
        _particlePool?.Dispose();
        _powderPool?.Dispose();
        _explodePool?.Dispose();
        _instance = null;
    }
}
