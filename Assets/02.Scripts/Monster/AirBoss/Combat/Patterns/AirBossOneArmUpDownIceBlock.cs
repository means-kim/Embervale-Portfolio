using UnityEngine;

public class AirBossOneArmUpDownIceBlock : AirBossAttackBase
{
    [Header("보스 컴포넌트")]
    [SerializeField] private AirBossAnimation _animation;

    [SerializeField] private AirBossController _controller;

    [Header("플레이어 위치")]
    [SerializeField] private Transform _player;

    [Header("텔레그래프 설정")]
    [SerializeField] private TelegraphType _telegraphType;

    [Header("텔레그래프 프리팹의 기본 크기에 곱할 배수")]
    [SerializeField] private Vector3 _telegraphScaleMultiplier = Vector3.one;

    [Header("아이스 블록 생성 위치")]
    [SerializeField] private Transform _iceBlockPoint;

    private AirBossAttack _iceBlock;
    private Telegraph _telegraph;
    private Vector3 _attackPosition;

    private bool _isRunning = false;
    private bool _isWarningShown = false;
    private bool _hasCast = false;

    private void Awake()
    {
        if (_player != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }

    protected override void BeginAttackCore()
    {
        if (_controller == null)
        {
            Debug.LogError("보스 컨트롤러를 연결하세요.", this);
            return;
        }

        if (_animation == null || _player == null)
        {
            Debug.Log("공격에 필요한 참조를 연결하세요.", this);
            _controller.OnActionFinished();
            return;
        }

        AirBossAttackPool attackPool = AirBossAttackPool.Instance;
        TelegraphPool telegraphPool = TelegraphPool.Instance;

        if (attackPool == null || telegraphPool == null)
        {
            _controller.OnActionFinished();
            return;
        }

        _iceBlock = attackPool.GetAttack(AirBossAttackType.IceBlockBezier, this);

        if (_iceBlock == null)
        {
            _controller.OnActionFinished();
            return;
        }

        _telegraph = telegraphPool.GetTelegraph(_telegraphType, this);

        if (_telegraph == null)
        {
            Cancel();
            _controller.OnActionFinished();
            return;
        }

        _isRunning = true;
        _animation.PlayOneArmUpDownIceBlockAnimation();
    }

    public override void ShowWarning()
    {
        if (!_isRunning) return;
        if (_isWarningShown) return;
        if (_hasCast) return;

        if (_player == null || _telegraph == null)
        {
            Finish();
            return;
        }

        _attackPosition = _player.position;
        _telegraph.Activate(_attackPosition, _telegraphScaleMultiplier);

        _isWarningShown = true;
    }

    public override void Cast()
    {
        if (!_isRunning) return;
        if (!_isWarningShown) return;
        if (_hasCast) return;

        if (_iceBlock == null)
        {
            Finish();
            return;
        }

        var movement =
            _iceBlock.GetComponent<AirBossAttackBezierMovement>();

        if (_iceBlockPoint == null || movement == null)
        {
            Debug.LogError("발사 위치 또는 베지어 이동 컴포넌트가 없습니다.", this);
            Finish();
            return;
        }

        _hasCast = true;

        movement.PrepareMove(_attackPosition);
        _iceBlock.Activate(_iceBlockPoint.position);

        _iceBlock = null;
        ReleaseTelegraph();
    }

    public override void Finish()
    {
        if (!_isRunning) return;

        Cancel();
        _controller.OnActionFinished();
    }

    protected override void CancelCore()
    {
        _isRunning = false;
        _isWarningShown = false;
        _hasCast = false;

        ReleaseTelegraph();

        if (_iceBlock != null)
        {
            _iceBlock.Release();
            _iceBlock = null;
        }
    }

    private void ReleaseTelegraph()
    {
        if (_telegraph == null) return;
        _telegraph.FadeOutAndRelease();
        _telegraph = null;
    }

    private void OnDisable()
    {
        Cancel();
    }
}