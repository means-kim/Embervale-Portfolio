using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;
    public static CameraManager Instance => _instance;
    [SerializeField] private GameObject _player;
    [SerializeField] private CinemachineCamera _defaultCamera;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    private CinemachineCamera _currentCamera;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
        ChangeCamera(_defaultCamera);
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    public static void SwithCamera(CinemachineCamera camera)
    {
        if (_instance == null)
        {
            return;
        }

        if (camera == null)
        {
            camera = _instance._defaultCamera;
        }

        _instance.ChangeCamera(camera);
    }

    private void ChangeCamera(CinemachineCamera nextCamera)
    {
        if (nextCamera == null || nextCamera == _currentCamera)
        {
            return;
        }

        nextCamera.gameObject.SetActive(true);
        if (_currentCamera != null)
        {
            _currentCamera.gameObject.SetActive(false);
        }

        _currentCamera = nextCamera;
    }

    public static void ShakeCamera(float strength)
    {
        if (_instance == null || _instance._currentCamera == null || _instance._impulseSource == null || strength <= 0f)
        {
            return;
        }

        Vector2 randomDirection = Random.insideUnitCircle;
        _instance._impulseSource.GenerateImpulseWithVelocity(
            new Vector3(randomDirection.x, randomDirection.y, 0f) * strength
        );
    }

    // 피격 등 개별 연출의 지속시간. 기존 호출의 Impulse 설정은 보존한다.
    public static void ShakeCamera(float strength, float duration)
    {
        if (_instance == null || _instance._currentCamera == null || _instance._impulseSource == null || strength <= 0f)
        {
            return;
        }

        // Impulse 신호는 Definition을 계속 참조하므로 요청마다 독립 사본을 사용한다.
        var source = _instance._impulseSource;
        var definition =
            JsonUtility.FromJson<CinemachineImpulseDefinition>(JsonUtility.ToJson(source.ImpulseDefinition));
        definition.ImpulseDuration = Mathf.Max(0.01f, duration);
        Vector2 direction = Random.insideUnitCircle.normalized;
        if (direction == Vector2.zero)
        {
            direction = Vector2.right;
        }

        definition.CreateEvent(source.transform.position, new Vector3(direction.x, direction.y, 0f) * strength);
    }
}