using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [FormerlySerializedAs("_jumpStartDuration")]
    [SerializeField, Min(0f), Tooltip("공중 점프 회전 표시 시간")] private float _doubleJumpDuration = 0.2f;
    [SerializeField, Min(0f)] private float _apexVelocityRange = 1.5f;
    [SerializeField, Min(0f), Tooltip("미세 들뜸 깜빡임 방지")] private float _apexMinAirTime = 0.15f;
    private Animator _animator;
    private string _currentClip;
    private bool _playingDoubleJump;
    private float _doubleJumpStartTime;
    private float _jumpEnterTime = -100f;
    private bool _chargeJumpLaunchVisual;
    private SpriteRenderer _sourceRenderer;
    private SpriteRenderer _chargeRenderer;
    private bool _previousForceRenderingOff;
    private Vector2 _chargeDirection;
    private MaterialPropertyBlock _visualProperties;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _visualProperties = new MaterialPropertyBlock();
        _sourceRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
    }
    public void PlayState(PlayerState state, bool isDoubleJump)
    {
        if (_chargeJumpLaunchVisual)
        {
            if (state == PlayerState.Death || state == PlayerState.Hit)
            {
                StopChargeJumpVisual();
            }
            else
            {
                PlayPhaseClip("Dash");
                return;
            }
        }
        switch (state)
        {
            case PlayerState.Idle:
                PlayClip("Idle");
                break;
            case PlayerState.Run:
                PlayClip("Run");
                break;
            case PlayerState.Jump:
                _jumpEnterTime = Time.time;
                _playingDoubleJump = false;
                PlayClip(GetComponent<Rigidbody2D>().linearVelocity.y < 0f ? "Fall" : "JumpRise");
                break;
            case PlayerState.Dash:
                PlayClip("Dash");
                break;
            case PlayerState.Attack:
                PlayClip(GetComponent<PlayerAttack>().CurrentAttackClip);
                break;
            case PlayerState.Death:
                PlayClip("Death");
                break;
            case PlayerState.Hit:
                PlayClip("Hit");
                break;
            case PlayerState.WallCling:
                PlayClip("WallCling");
                break;
            case PlayerState.Glide:
                PlayClip("Glide");
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(state));
        }
    }
    public void BeginJump(bool isDoubleJump)
    {
        if (_chargeJumpLaunchVisual) return;
        _jumpEnterTime = Time.time;
        _playingDoubleJump = isDoubleJump;
        _doubleJumpStartTime = Time.time;
        PlayClip(isDoubleJump ? "DoubleJump" : "JumpRise");
    }
    public void ResetJumpPhase() => _playingDoubleJump = false;
    public void PlayLand()
    {
        if (!_chargeJumpLaunchVisual) PlayPhaseClip("Idle");
    }
    /// <summary>대시의 이동 로직 없이 차지 점프 발사 연출만 재생합니다.</summary>
    public void PlayDirectionalDash(Vector2 direction) => PlayChargeJumpLaunch(direction);
    public void EndDirectionalDash() => EndChargeJumpLaunch();
    public void PlayChargeJumpLaunch(Vector2 direction)
    {
        if (!_chargeJumpLaunchVisual && _sourceRenderer != null)
            _previousForceRenderingOff = _sourceRenderer.forceRenderingOff;
        _chargeJumpLaunchVisual = true;
        _chargeDirection = direction;
        PlayClip("Dash");
        SyncChargeJumpVisual();
    }
    public void EndChargeJumpLaunch()
    {
        if (!_chargeJumpLaunchVisual) return;
        StopChargeJumpVisual();
        ResetJumpPhase();
        var controller = GetComponent<PlayerController>();
        if (controller != null && controller.CurrentState != PlayerState.Jump)
            PlayState(controller.CurrentState, false);
        else
            UpdateJumpPhase(GetComponent<Rigidbody2D>().linearVelocity.y);
    }
    private void OnDisable() => StopChargeJumpVisual();
    private void LateUpdate()
    {
        if (_chargeJumpLaunchVisual) SyncChargeJumpVisual();
    }

    // Animator 연결과 물리 루트는 유지하고, 표시용 자식 스프라이트만 기울입니다.
    private void SyncChargeJumpVisual()
    {
        if (_sourceRenderer == null) return;
        if (_chargeRenderer == null)
        {
            var visual = new GameObject("ChargeJumpLaunchVisual");
            visual.transform.SetParent(_sourceRenderer.transform, false);
            _chargeRenderer = visual.AddComponent<SpriteRenderer>();
        }
        _sourceRenderer.forceRenderingOff = true;
        _chargeRenderer.gameObject.layer = _sourceRenderer.gameObject.layer;
        _chargeRenderer.enabled = _sourceRenderer.enabled && !_previousForceRenderingOff;
        _chargeRenderer.sprite = _sourceRenderer.sprite;
        _chargeRenderer.sharedMaterial = _sourceRenderer.sharedMaterial;
        _chargeRenderer.color = _sourceRenderer.color;
        _chargeRenderer.flipX = _sourceRenderer.flipX;
        _chargeRenderer.flipY = _sourceRenderer.flipY;
        _chargeRenderer.sortingLayerID = _sourceRenderer.sortingLayerID;
        _chargeRenderer.sortingOrder = _sourceRenderer.sortingOrder;
        _chargeRenderer.maskInteraction = _sourceRenderer.maskInteraction;
        _chargeRenderer.drawMode = _sourceRenderer.drawMode;
        _chargeRenderer.size = _sourceRenderer.size;
        _chargeRenderer.spriteSortPoint = _sourceRenderer.spriteSortPoint;
        // Play 중 스크립트 재로드 시 Awake가 다시 실행되지 않은 경우도 처리합니다.
        _visualProperties ??= new MaterialPropertyBlock();
        _sourceRenderer.GetPropertyBlock(_visualProperties);
        _chargeRenderer.SetPropertyBlock(_visualProperties);
        Vector3 localDirection = _sourceRenderer.transform.InverseTransformVector(_chargeDirection);
        if (_sourceRenderer.flipX) localDirection = -localDirection;
        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
        _chargeRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public SpriteRenderer GetChargeJumpVisualRenderer(SpriteRenderer original)
    {
        if (!_chargeJumpLaunchVisual || original != _sourceRenderer) return original;
        SyncChargeJumpVisual();
        return _chargeRenderer != null ? _chargeRenderer : original;
    }

    private void StopChargeJumpVisual()
    {
        if (_chargeJumpLaunchVisual && _sourceRenderer != null)
            _sourceRenderer.forceRenderingOff = _previousForceRenderingOff;
        _chargeJumpLaunchVisual = false;
        if (_chargeRenderer != null) _chargeRenderer.enabled = false;
    }
    private void OnDestroy()
    {
        if (_chargeRenderer != null) Destroy(_chargeRenderer.gameObject);
    }
    public void Tick(PlayerState state, bool landingLocked, float verticalVelocity)
    {
        if (_chargeJumpLaunchVisual)
        {
            PlayPhaseClip("Dash");
            return;
        }
        if (landingLocked) return;
        switch (state)
        {
            case PlayerState.Jump:
                UpdateJumpPhase(verticalVelocity);
                break;
            case PlayerState.Dash:
                PlayPhaseClip("Dash");
                break;
        }
    }
    private void UpdateJumpPhase(float verticalVelocity)
    {
        if (_playingDoubleJump)
        {
            float elapsed = Time.time - _doubleJumpStartTime;
            if (_doubleJumpDuration > 0f && elapsed < _doubleJumpDuration)
            {
                _animator.Play("DoubleJump", 0, Mathf.Min(elapsed / _doubleJumpDuration, 0.9999f));
                return;
            }
            _playingDoubleJump = false;
        }
        if (verticalVelocity > _apexVelocityRange)
        {
            PlayPhaseClip("JumpRise");
        }
        else if (verticalVelocity < -_apexVelocityRange)
        {
            PlayPhaseClip("Fall");
        }
        else
        {
            if (Time.time - _jumpEnterTime < _apexMinAirTime) return;
            // 정점 진입/이탈 속도에 맞춰 Rise와 Fall 사이의 자세를 이어 줍니다.
            _currentClip = "JumpApex";
            float progress = Mathf.InverseLerp(_apexVelocityRange, -_apexVelocityRange, verticalVelocity);
            _animator.Play("JumpApex", 0, Mathf.Min(progress, 0.9999f));
        }
    }
    private void PlayClip(string clipName)
    {
        _currentClip = clipName;
        _animator.Play(clipName, 0, 0f);
    }
    private void PlayPhaseClip(string clipName)
    {
        if (_currentClip == clipName) return;
        _currentClip = clipName;
        _animator.Play(clipName);
    }
}
