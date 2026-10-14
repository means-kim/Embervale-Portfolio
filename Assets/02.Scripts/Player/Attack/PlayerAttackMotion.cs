using UnityEngine;

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
        bool rising = _body.linearVelocity.y > 0.01f;
        if (horizontalDuration > 0f)
        {
            _move.SetMovementLocked(_owner, true);
            _horizontalTimer = horizontalDuration;
            _horizontalHeld = true;
        }
        if (inAir && !rising && verticalDuration > 0f)
        {
            _frozenY = _body.position.y;
            _verticalTimer = verticalDuration;
            _verticalHeld = true;
        }
        if (!rising)
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
        if (!_horizontalHeld)
        {
            return;
        }
        _horizontalTimer -= deltaTime;
        if (_horizontalTimer <= 0f)
        {
            UnlockHorizontal();
        }
    }

    public void FixedTick(bool grounded, float deltaTime)
    {
        if (!_verticalHeld)
        {
            return;
        }
        if (grounded)
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
        if (_verticalTimer <= 0f)
        {
            CancelVertical();
        }
    }

    public void EndAttack()
    {
        if (!_gravityReduced)
        {
            return;
        }
        _body.gravityScale = _originalGravity;
        _gravityReduced = false;
    }

    public void UnlockHorizontal()
    {
        _horizontalTimer = 0f;
        _horizontalHeld = false;
        _move.SetMovementLocked(_owner, false);
    }

    public void CancelVertical()
    {
        _verticalTimer = 0f;
        _verticalHeld = false;
    }
}
