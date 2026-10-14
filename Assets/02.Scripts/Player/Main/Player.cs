using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerRig))]
public class Player : MonoBehaviour
{
    public event System.Action OnRestarted;
    private Rigidbody2D _rigidbody2D;
    private Vector3 _respawnPosition;
    private RespawnPoint _respawnPoint;
    private PlayerGlide _glide;
    private PlayerChargeJump _chargeJump;
    private PlayerAttack _attack;
    private PlayerDash _dash;
    private PlayerDashGhost _dashGhost;
    private PlayerJump _jump;
    private PlayerLandEffect _landEffect;
    private PlayerWall _wall;
    private PlayerMove _move;
    private Health _health;
    private PlayerHit _hit;
    private PlayerController _controller;
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _rigidbody2D = rig.Body;
        _glide = rig.Glide;
        _chargeJump = rig.ChargeJump;
        _attack = rig.Attack;
        _dash = rig.Dash;
        _dashGhost = rig.DashGhost;
        _jump = rig.Jump;
        _landEffect = rig.LandEffect;
        _wall = rig.Wall;
        _move = rig.Move;
        _health = rig.Health;
        _hit = rig.Hit;
        _controller = rig.Controller;
        _respawnPosition = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Border"))
        {
            PlayerDeath();
        }
    }
    private void PlayerDeath()
    {
        if (global::Respone.Instance != null)
        {
            global::Respone.Instance.ResponePlayer(this, _respawnPosition);
        }
        else
        {
            MoveToCheckpoint(_respawnPosition);
        }
    }

    public void MoveToCheckpoint(Vector3 position)
    {
        _glide.ResetForRespawn();
        _chargeJump.CancelChargeJump();
        Vector3 positionDelta = position - transform.position;
        _attack.CancelAttack();
        _dash.CancelDash();
        _dashGhost.ResetForRespawn();
        _jump.ResetForRespawn();
        _landEffect.ResetForRespawn();
        _wall.ResetForRespawn();
        _move.ResetForRespawn();

        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
        transform.position = position;

        foreach (CinemachineCamera camera in FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            camera.OnTargetObjectWarped(transform, positionDelta);
        }

        if (_respawnPoint != null && (position - _respawnPoint.transform.position).sqrMagnitude < 0.0001f)
        {
            _respawnPoint.RestoreView(transform);
        }
    }

    public void RestartAt(Vector3 position)
    {
        _respawnPoint?.PrepareForRetry();
        MoveToCheckpoint(position);
        _health.ReviveFull();
        _hit.ResetForRespawn();
        _dash.ResetForRespawn();
        _attack.ResetForRespawn();
        _controller.ResetForRespawn();
        OnRestarted?.Invoke();
    }
    public void SetRespawnPoint(Vector3 position)
    {
        _respawnPosition = position;
        _respawnPoint = null;
    }

    public void SetRespawnPoint(RespawnPoint point)
    {
        if (point == null)
        {
            return;
        }
        _respawnPoint = point;
        _respawnPosition = point.transform.position;
    }
}
