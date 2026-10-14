using UnityEngine;

/// <summary>
/// 플레이어 릭의 배선표. 같은 오브젝트에 붙은 전 컴포넌트를 Awake에서 1회만 찾아낸다.
/// 각 능력은 GetComponent를 여러 번 호출하는 대신
/// <c>PlayerRig.Resolve(gameObject)</c> 1회로 필요한 참조를 모두 가져간다.
/// 릭 전체 구성 선언(RequireComponent)도 이 파일이 유일하게 들고 있다.
/// 프리팹에 미리 붙어 있지 않아도 Resolve가 실행 중에 자동 부착한다.
/// </summary>
[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerWall))]
[RequireComponent(typeof(PlayerHit))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerGlide))]
[RequireComponent(typeof(PlayerChargeJump))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerJumpPhaseAnimator))]
[RequireComponent(typeof(PlayerChargeJumpVisual))]
[RequireComponent(typeof(PlayerDashGhost))]
[RequireComponent(typeof(PlayerFootsteps))]
[RequireComponent(typeof(PlayerLandEffect))]
[RequireComponent(typeof(PlayerLevel))]
[RequireComponent(typeof(BuffPool))]
public sealed class PlayerRig : MonoBehaviour
{
    public Rigidbody2D Body { get; private set; }
    public Collider2D PlayerCollider { get; private set; }
    public Animator Animator { get; private set; }
    public Health Health { get; private set; }
    public Game.Upgrades.PlayerUpgrades Upgrades { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerInput Input { get; private set; }
    public PlayerMove Move { get; private set; }
    public PlayerJump Jump { get; private set; }
    public PlayerDash Dash { get; private set; }
    public PlayerWall Wall { get; private set; }
    public PlayerHit Hit { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerGlide Glide { get; private set; }
    public PlayerChargeJump ChargeJump { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }
    public PlayerJumpPhaseAnimator JumpPhase { get; private set; }
    public PlayerChargeJumpVisual ChargeVisual { get; private set; }
    public PlayerDashGhost DashGhost { get; private set; }
    public PlayerFootsteps Footsteps { get; private set; }
    public PlayerLandEffect LandEffect { get; private set; }
    public PlayerLevel Level { get; private set; }
    public BuffPool Buffs { get; private set; }

    public static PlayerRig Resolve(GameObject go) => go.GetComponent<PlayerRig>() ?? go.AddComponent<PlayerRig>();

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<Collider2D>();
        Animator = GetComponent<Animator>();
        Health = GetComponent<Health>();
        Upgrades = GetComponent<Game.Upgrades.PlayerUpgrades>();
        Controller = GetComponent<PlayerController>();
        Input = GetComponent<PlayerInput>();
        Move = GetComponent<PlayerMove>();
        Jump = GetComponent<PlayerJump>();
        Dash = GetComponent<PlayerDash>();
        Wall = GetComponent<PlayerWall>();
        Hit = GetComponent<PlayerHit>();
        Attack = GetComponent<PlayerAttack>();
        Glide = GetComponent<PlayerGlide>();
        ChargeJump = GetComponent<PlayerChargeJump>();
        PlayerAnimator = GetComponent<PlayerAnimator>();
        JumpPhase = GetComponent<PlayerJumpPhaseAnimator>();
        ChargeVisual = GetComponent<PlayerChargeJumpVisual>();
        DashGhost = GetComponent<PlayerDashGhost>();
        Footsteps = GetComponent<PlayerFootsteps>();
        LandEffect = GetComponent<PlayerLandEffect>();
        Level = GetComponent<PlayerLevel>();
        Buffs = GetComponent<BuffPool>();
    }
}
