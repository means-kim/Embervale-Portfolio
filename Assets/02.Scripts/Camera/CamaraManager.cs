using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
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
        if (_instance == this)
        {
            _instance = null;
        }
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
        if (_instance == null || _instance._currentCamera == null)
            return;

        Vector2 randomDirection = Random.insideUnitCircle;

        _instance._impulseSource.GenerateImpulseWithVelocity(
            new Vector3(randomDirection.x, randomDirection.y, 0f) * strength
        );
    }

}
