using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerController))]
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
    private bool _inApex;
    private float _lastApexProgress = -1f;
    private const float ApexProgressEpsilon = 0.02f;
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
                if (GetComponent<Rigidbody2D>().linearVelocity.y < 0f)
                {
                    PlayClip("Fall");
                }
                else
                {
                    PlayClip("JumpRise");
                }
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
        if (_chargeJumpLaunchVisual)
        {
            return;
        }
        _jumpEnterTime = Time.time;
        _playingDoubleJump = isDoubleJump;
        _doubleJumpStartTime = Time.time;
        _inApex = false;
        _lastApexProgress = -1f;
        if (isDoubleJump)
        {
            PlayClip("DoubleJump");
        }
        else
        {
            PlayClip("JumpRise");
        }
        if (isDoubleJump)
        {
            FitDoubleJumpSpeed();
        }
    }
    public void ResetJumpPhase()
    {
        _playingDoubleJump = false;
        ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
    }
    private void FitDoubleJumpSpeed()
    {
        if (_doubleJumpDuration <= 0f)
        {
            _animator.speed = 1f;
            return;
        }
        var controller = _animator.runtimeAnimatorController;
        if (controller != null)
        {
            foreach (var clip in controller.animationClips)
            {
                if (clip != null && clip.length > 0f && clip.name.Contains("DoubleJump"))
                {
                    _animator.speed = clip.length / _doubleJumpDuration;
                    return;
                }
            }
        }
        _animator.speed = 1f;
    }
    public void PlayLand()
    {
        if (!_chargeJumpLaunchVisual)
        {
            PlayPhaseClip("Idle");
        }
    }
    public void PlayDirectionalDash(Vector2 direction) => PlayChargeJumpLaunch(direction);
    public void EndDirectionalDash() => EndChargeJumpLaunch();
    public void PlayChargeJumpLaunch(Vector2 direction)
    {
        if (!_chargeJumpLaunchVisual && _sourceRenderer != null)
        {
            _previousForceRenderingOff = _sourceRenderer.forceRenderingOff;
        }
        _chargeJumpLaunchVisual = true;
        _chargeDirection = direction;
        PlayClip("Dash");
        SyncChargeJumpVisual();
    }
    public void EndChargeJumpLaunch()
    {
        if (!_chargeJumpLaunchVisual)
        {
            return;
        }
        StopChargeJumpVisual();
        ResetJumpPhase();
        var controller = GetComponent<PlayerController>();
        if (controller.CurrentState != PlayerState.Jump)
        {
            PlayState(controller.CurrentState, false);
        }
        else
        {
            UpdateJumpPhase(GetComponent<Rigidbody2D>().linearVelocity.y);
        }
    }
    private void OnDisable()
    {
        StopChargeJumpVisual();
        _animator.speed = 1f;
    }
    private void LateUpdate()
    {
        if (_chargeJumpLaunchVisual)
        {
            SyncChargeJumpVisual();
        }
    }

    private void SyncChargeJumpVisual()
    {
        if (_sourceRenderer == null)
        {
            return;
        }
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
        _visualProperties ??= new MaterialPropertyBlock();
        _sourceRenderer.GetPropertyBlock(_visualProperties);
        _chargeRenderer.SetPropertyBlock(_visualProperties);
        Vector3 localDirection = _sourceRenderer.transform.InverseTransformVector(_chargeDirection);
        if (_sourceRenderer.flipX)
        {
            localDirection = -localDirection;
        }
        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
        _chargeRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public SpriteRenderer GetChargeJumpVisualRenderer(SpriteRenderer original)
    {
        if (!_chargeJumpLaunchVisual || original != _sourceRenderer)
        {
            return original;
        }
        SyncChargeJumpVisual();
        if (_chargeRenderer != null)
        {
            return _chargeRenderer;
        }
        return original;
    }

    private void StopChargeJumpVisual()
    {
        if (_chargeJumpLaunchVisual && _sourceRenderer != null)
        {
            _sourceRenderer.forceRenderingOff = _previousForceRenderingOff;
        }
        _chargeJumpLaunchVisual = false;
        if (_chargeRenderer != null)
        {
            _chargeRenderer.enabled = false;
        }
    }
    private void OnDestroy()
    {
        if (_chargeRenderer != null)
        {
            Destroy(_chargeRenderer.gameObject);
        }
    }
    public void Tick(PlayerState state, bool landingLocked, float verticalVelocity)
    {
        if (_chargeJumpLaunchVisual)
        {
            PlayPhaseClip("Dash");
            return;
        }
        if (landingLocked)
        {
            return;
        }
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
                return;
            }
            _playingDoubleJump = false;
            _animator.speed = 1f;
            _currentClip = null;
        }
        if (verticalVelocity > _apexVelocityRange)
        {
            ExitApex();
            PlayPhaseClip("JumpRise");
        }
        else if (verticalVelocity < -_apexVelocityRange)
        {
            ExitApex();
            PlayPhaseClip("Fall");
        }
        else
        {
            if (Time.time - _jumpEnterTime < _apexMinAirTime)
            {
                return;
            }
            float progress = Mathf.Clamp01(Mathf.InverseLerp(_apexVelocityRange, -_apexVelocityRange, verticalVelocity));
            progress = Mathf.Min(progress, 0.9999f);
            if (!_inApex || _currentClip != "JumpApex")
            {
                _inApex = true;
                _currentClip = "JumpApex";
                _lastApexProgress = progress;
                _animator.speed = 0f;
                _animator.Play("JumpApex", 0, progress);
            }
            else if (Mathf.Abs(progress - _lastApexProgress) >= ApexProgressEpsilon)
            {
                _lastApexProgress = progress;
                _animator.Play("JumpApex", 0, progress);
            }
        }
    }
    private void ExitApex()
    {
        if (!_inApex)
        {
            return;
        }
        _inApex = false;
        _lastApexProgress = -1f;
        if (_animator.speed == 0f)
        {
            _animator.speed = 1f;
        }
    }
    private void PlayClip(string clipName)
    {
        ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
        _currentClip = clipName;
        _animator.Play(clipName, 0, 0f);
    }
    private void PlayPhaseClip(string clipName)
    {
        if (_currentClip == clipName)
        {
            return;
        }
        ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
        _currentClip = clipName;
        _animator.Play(clipName);
    }
}
