using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class EliteEncounterCinematic : CinematicSequence
{
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _eliteMonster;

    [SerializeField, Min(0.01f)]
    private float _moveTime = 0.2f;

    [SerializeField, Min(0f)]
    private float _holdTime = 2f;

    private CinemachineBrain _brain;
    private Vector3 _originalPosition;

    private bool _playing;
    private bool _cameraCaptured;
    private bool _brainWasEnabled;

    public override bool TryPlay()
    {
        if (!Application.isPlaying ||
            !isActiveAndEnabled ||
            _playing ||
            _cinematicManager == null ||
            _camera == null ||
            !_camera.isActiveAndEnabled ||
            _eliteMonster == null)
        {
            return false;
        }

        _brain = _camera.GetComponent<CinemachineBrain>();
        _cameraCaptured = false;
        _playing = true;

        bool accepted = _cinematicManager.Play(EncounterRoutine(), HandleCompleted, HandleCancelled);

        if (!accepted)
        {
            _playing = false;
        }

        return accepted;
    }

    private IEnumerator EncounterRoutine()
    {
        yield return null;

        // 게이트의 방 카메라 전환이 끝날 때까지
        while (_brain != null && _brain.enabled && _brain.IsBlending)
        {
            yield return null;
        }

        if (_camera == null || _eliteMonster == null)
        {
            Cancel();
            yield break;
        }

        _originalPosition = _camera.transform.position;
        _cameraCaptured = true;

        if (_brain != null)
        {
            _brainWasEnabled = _brain.enabled;
            _brain.enabled = false;
        }

        Vector3 target = _eliteMonster.position;
        target.y = _eliteMonster.position.y + 10;
        target.z = _originalPosition.z;

        yield return MoveCamera(target);
        yield return new WaitForSecondsRealtime(_holdTime);
        yield return MoveCamera(_originalPosition);
    }

    private IEnumerator MoveCamera(Vector3 target)
    {
        Vector3 start = _camera.transform.position;
        float duration = Mathf.Max(0.01f, _moveTime);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (_camera == null)
            {
                Cancel();
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;

            _camera.transform.position = Vector3.Lerp(
                start,
                target,
                Mathf.Clamp01(elapsed / duration));

            yield return null;
        }

        _camera.transform.position = target;
    }

    private void RestoreCamera()
    {
        if (_cameraCaptured)
        {
            if (_camera != null)
            {
                _camera.transform.position = _originalPosition;
            }

            if (_brain != null)
            {
                _brain.enabled = _brainWasEnabled;
            }
        }

        _cameraCaptured = false;
        _playing = false;
    }

    private void HandleCompleted()
    {
        RestoreCamera();
        NotifyCompleted();
    }

    private void HandleCancelled()
    {
        RestoreCamera();
        NotifyCancelled();
    }

    public override void Cancel()
    {
        if (!_playing)
        {
            return;
        }

        if (_cinematicManager != null &&
            _cinematicManager.IsPlaying)
        {
            _cinematicManager.Cancel();
        }
        else
        {
            HandleCancelled();
        }
    }

    private void OnDisable()
    {
        Cancel();
    }
}