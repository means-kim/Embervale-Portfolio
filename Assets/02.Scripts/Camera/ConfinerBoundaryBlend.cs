using Unity.Cinemachine;
using UnityEngine;

// Smooth only the discontinuity caused by replacing a Confiner boundary.
// Ordinary follow damping and camera impulses continue through Cinemachine.
[DisallowMultipleComponent]
public sealed class ConfinerBoundaryBlend : CinemachineExtension
{
    private Vector3 _lastPosition;
    private Vector3 _positionVelocity;
    private float _duration;
    private float _elapsed;
    private bool _hasLastPosition;
    private bool _pending;
    private bool _blending;

    public void BeginTransition(float duration)
    {
        _duration = Mathf.Max(0f, duration);
        _pending = _hasLastPosition && _duration > 0f;
        _positionVelocity = Vector3.zero;
        _blending = false;
        _elapsed = 0f;
    }

    public void ResetBlend()
    {
        _pending = false;
        _positionVelocity = Vector3.zero;
        _blending = false;
        _hasLastPosition = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetBlend();
    }

    private void OnDisable() => ResetBlend();

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage,
        ref CameraState state, float deltaTime)
    {
        // Finalize runs after the Confiner's Body correction, independent of component order.
        if (stage != CinemachineCore.Stage.Finalize) return;
        if (deltaTime < 0f || !vcam.PreviousStateIsValid) ResetBlend();
        Vector3 position = state.GetCorrectedPosition();
        if (_pending)
        {
            _elapsed = 0f;
            _pending = false;
            _blending = true;
        }

        if (_blending && deltaTime > 0f)
        {
            // The Confiner can keep correcting over several frames. Smooth its changing
            // result, rather than fading only the first frame's displacement.
            float progress = Mathf.Clamp01(_elapsed / _duration);
            float smoothTime = Mathf.Max(0.01f, _duration * 0.33f * (1f - progress));
            Vector3 blended = Vector3.SmoothDamp(_lastPosition, position,
                ref _positionVelocity, smoothTime, Mathf.Infinity, deltaTime);
            state.PositionCorrection += blended - position;
            _elapsed += deltaTime;
            if (_elapsed >= _duration) _blending = false;
        }

        _lastPosition = state.GetCorrectedPosition();
        _hasLastPosition = true;
    }

    public override void OnTargetObjectWarped(
        CinemachineVirtualCameraBase vcam, Transform target, Vector3 positionDelta)
    {
        base.OnTargetObjectWarped(vcam, target, positionDelta);
        if (target == vcam.Follow) ResetBlend();
    }
}