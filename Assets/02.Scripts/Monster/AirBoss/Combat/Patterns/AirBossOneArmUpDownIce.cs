using UnityEngine;

public class AirBossOneArmUpDownIce : AirBossAttackBase
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

    [Header("공격 X 축 간격")]
    [SerializeField] private float _offSetX = 3f;

    private AirBossAttack[] _icicle = new AirBossAttack[3];
    private Telegraph[] _telegraph = new Telegraph[3];
    private Vector3[] _attackPosition = new Vector3[3];

    private bool _isRunning = false;
    private bool _isWarnigShow = false;
    private bool _hasCast = false;

    public override void BeginAttack()
    {
        Cancel();

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

        for (int i = 0; i < _icicle.Length; i++)
        {
            _icicle[i] = attackPool.GetAttack(AirBossAttackType.Icicle);
            _telegraph[i] = telegraphPool.GetTelegraph(_telegraphType);

            if (_icicle[i] == null || _telegraph[i] == null)
            {
                Cancel();
                _controller.OnActionFinished();
                return;
            }
        }

        _isRunning = true;
        _animation.PlayBothArmsIceThrow();
    }

    public override void ShowWarning()
    {
        if (!_isRunning) return;
        if (_isWarnigShow) return;
        if (_hasCast) return;

        if (_player == null || _telegraph == null)
        {
            Finish();
            return;
        }

        Vector3 centerPosition = _player.position;
        _attackPosition[0] = centerPosition + Vector3.left * _offSetX;
        _attackPosition[1] = centerPosition;
        _attackPosition[2] = centerPosition + Vector3.right * _offSetX;

        for (int i = 0; i < _icicle.Length; i++)
        {
            _telegraph[i].Activate(_attackPosition[i], _telegraphScaleMultiplier);
        }

        _isWarnigShow = true;
    }

    public override void Cast()
    {
        if (!_isRunning) return;
        if (!_isWarnigShow) return;
        if (_hasCast) return;

        if (_icicle == null)
        {
            Finish();
            return;
        }

        _hasCast = true;

        for (int i = 0; i < _icicle.Length; i++)
        {
            if (_icicle[i] == null) continue;

            _icicle[i].Activate(_attackPosition[i]);
            _icicle[i] = null;
        }

        ReleaseTelegraph();
    }

    public override void Finish()
    {
        if (!_isRunning) return;

        Cancel();
        _controller.OnActionFinished();
    }

    public override void Cancel()
    {
        _isRunning = false;
        _isWarnigShow = false;
        _hasCast = false;

        ReleaseTelegraph();

        for (int i = 0; i < _icicle.Length; i++)
        {
            if (_icicle[i] == null) continue;

            _icicle[i].Release();
            _icicle[i] = null;
        }
    }

    private void ReleaseTelegraph()
    {
        for (int i = 0; i < _telegraph.Length; i++)
        {
            if (_telegraph[i] == null) continue;
            _telegraph[i].Release();
            _telegraph[i] = null;
        }
    }

    private void OnDisable()
    {
        Cancel();
    }
}