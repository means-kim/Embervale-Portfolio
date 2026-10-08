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

    [Header("Y축 오프셋")]
    [SerializeField] private float _offSetY = 8f;

    private AirBossAttack[] _icicles = new AirBossAttack[3];
    private Telegraph[] _telegraphs = new Telegraph[3];
    private Vector3[] _attackPositions = new Vector3[3];

    private bool _isRunning = false;
    private bool _isWarningShown = false;
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

        for (int i = 0; i < _icicles.Length; i++)
        {
            _icicles[i] = attackPool.GetAttack(AirBossAttackType.Icicle);
            _telegraphs[i] = telegraphPool.GetTelegraph(_telegraphType);

            if (_icicles[i] == null || _telegraphs[i] == null)
            {
                Cancel();
                _controller.OnActionFinished();
                return;
            }
        }

        _isRunning = true;
        _animation.PlayOneArmUpDownIceAnimation();
    }

    public override void ShowWarning()
    {
        if (!_isRunning) return;
        if (_isWarningShown) return;
        if (_hasCast) return;

        if (_player == null || _telegraphs == null)
        {
            Finish();
            return;
        }

        Vector3 centerPosition = _player.position;
        _attackPositions[0] = centerPosition + Vector3.left * _offSetX;
        _attackPositions[1] = centerPosition;
        _attackPositions[2] = centerPosition + Vector3.right * _offSetX;

        for (int i = 0; i < _icicles.Length; i++)
        {
            _telegraphs[i].Activate(_attackPositions[i], _telegraphScaleMultiplier);
        }

        _isWarningShown = true;
    }

    public override void Cast()
    {
        if (!_isRunning) return;
        if (!_isWarningShown) return;
        if (_hasCast) return;

        if (_icicles == null)
        {
            Finish();
            return;
        }

        _hasCast = true;

        for (int i = 0; i < _icicles.Length; i++)
        {
            if (_icicles[i] == null) continue;

            Vector3 spawnPosition = _attackPositions[i] + Vector3.up * _offSetY;

            _icicles[i].Activate(spawnPosition);
            _icicles[i] = null;
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
        _isWarningShown = false;
        _hasCast = false;

        ReleaseTelegraph();

        for (int i = 0; i < _icicles.Length; i++)
        {
            if (_icicles[i] == null) continue;

            _icicles[i].Release();
            _icicles[i] = null;
        }
    }

    private void ReleaseTelegraph()
    {
        for (int i = 0; i < _telegraphs.Length; i++)
        {
            if (_telegraphs[i] == null) continue;
            _telegraphs[i].FadeOutAndRelease();
            _telegraphs[i] = null;
        }
    }

    private void OnDisable()
    {
        Cancel();
    }
}