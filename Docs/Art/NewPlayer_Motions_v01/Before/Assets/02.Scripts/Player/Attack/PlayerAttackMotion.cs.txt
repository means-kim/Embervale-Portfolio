using UnityEngine;

// 공격 중 물리 제어. 잠금 소유자는 기존 PlayerAttack 컴포넌트를 유지한다.
internal sealed class PlayerAttackMotion
{
    private readonly PlayerAttack _owner;
    private readonly PlayerMove _move;
    private readonly Rigidbody2D _body;
    private float _originalGravity;
    private bool _gravityReduced;
    private float _horizontalTimer;
    private bool _horizontalHeld;
    private float _verticalTimer;
    private bool _verticalHeld;
    private float _frozenY;

    public PlayerAttackMotion(PlayerAttack owner, PlayerMove move, Rigidbody2D body)
    {
        _owner = owner;
        _move = move;
        _body = body;
    }

    public void Begin(bool inAir, float horizontalDuration, float verticalDuration, float gravityMultiplier)
    {
        bool rising = _body != null && _body.linearVelocity.y > 0.01f;
        if (horizontalDuration > 0f && _move != null)
        {
            _move.SetMovementLocked(_owner, true);
            _horizontalTimer = horizontalDuration;
            _horizontalHeld = true;
        }
        if (inAir && !rising && verticalDuration > 0f && _body != null)
        {
            _frozenY = _body.position.y;
            _verticalTimer = verticalDuration;
            _verticalHeld = true;
        }
        if (!rising && _body != null)
        {
            if (!_gravityReduced)
            {
                _originalGravity = _body.gravityScale;
                _gravityReduced = true;
            }
            _body.gravityScale = _originalGravity * gravityMultiplier;
        }
    }

    public void Tick(float deltaTime)
    {
        if (!_horizontalHeld) return;
        _horizontalTimer -= deltaTime;
        if (_horizontalTimer <= 0f) UnlockHorizontal();
    }

    public void FixedTick(bool grounded, float deltaTime)
    {
        if (!_verticalHeld) return;
        if (_body == null || grounded)
        {
            CancelVertical();
            return;
        }
        Vector2 velocity = _body.linearVelocity;
        velocity.y = 0f;
        _body.linearVelocity = velocity;
        Vector2 position = _body.position;
        position.y = _frozenY;
        _body.position = position;
        _verticalTimer -= deltaTime;
        if (_verticalTimer <= 0f) CancelVertical();
    }

    public void EndAttack(bool continueCombo)
    {
        // 수평·수직 제한은 각 타이머로 종료하고, 연속 콤보는 중력 감소를 유지한다.
        if (!_gravityReduced || continueCombo || _body == null) return;
        _body.gravityScale = _originalGravity;
        _gravityReduced = false;
    }

    public void UnlockHorizontal()
    {
        _horizontalTimer = 0f;
        _horizontalHeld = false;
        if (_move != null) _move.SetMovementLocked(_owner, false);
    }

    public void CancelVertical()
    {
        _verticalTimer = 0f;
        _verticalHeld = false;
    }
}
