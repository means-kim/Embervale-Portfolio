using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;
    public static CameraManager instance => _instance;

    [SerializeField] private GameObject _player;
    [SerializeField] private CinemachineCamera _defaultCamera;

    private CinemachineCamera _currentCamera;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
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
        if (nextCamera == _currentCamera)
        {
            return;
        }

        if (nextCamera.Follow == null)
        {
            nextCamera.Follow = _player.transform;
        }

        nextCamera.gameObject.SetActive(true);

        if (_currentCamera != null)
        {
            _currentCamera.gameObject.SetActive(false);
        }

        _currentCamera = nextCamera;
    }

}
