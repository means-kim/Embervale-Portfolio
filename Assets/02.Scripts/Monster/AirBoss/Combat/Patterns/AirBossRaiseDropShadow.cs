using UnityEngine;

public class AirBossRaiseDropShadow : AirBossAttackBase
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

    private AirBossAttack _blizzard;
    private Telegraph _telegraph;
    private Vector3 _attackPosition;

    private bool _isRunning = false;
    private bool _isWarningShown = false;
    private bool _hasCast = false;

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

        _blizzard = attackPool.GetAttack(AirBossAttackType.Blizzard, this);

        if (_blizzard == null)
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
        _animation.PlayRaiseDropShadowAnimation();
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

        if (_blizzard == null)
        {
            Finish();
            return;
        }

        _hasCast = true;

        _blizzard.Activate(_attackPosition);

        _blizzard = null;
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

        if (_blizzard != null)
        {
            _blizzard.Release();
            _blizzard = null;
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