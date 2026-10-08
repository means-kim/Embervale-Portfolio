using UnityEngine;
using System.Collections;

public class AirBossOneArmUpDownIcePhase3 : AirBossAttackBase
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
    [SerializeField] private float _offSetX = 20f;

    [Header("Y축 오프셋")]
    [SerializeField] private float _offSetY = 50f;

    [Header("코루틴에 사용")]
    [SerializeField] private float _warningInterval = 0.2f;

    [SerializeField] private float _waitAfterWarnings = 0.5f;
    [SerializeField] private float _attackInterval = 0.2f;

    private Coroutine _sequence;

    private const int AttackCount = 5;

    private AirBossAttack[] _icicles = new AirBossAttack[AttackCount];
    private Telegraph[] _telegraphs = new Telegraph[AttackCount];
    private Vector3[] _attackPositions = new Vector3[AttackCount];

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
        _animation.PlayOneArmUpDownIcePhase3Animation();
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

        _isWarningShown = true;
        Vector3 centerPosition = _player.position;

        _sequence = StartCoroutine(AttackSequence(centerPosition));
    }

    private IEnumerator AttackSequence(Vector3 centerPosition)
    {
        _attackPositions[0] = centerPosition;
        _attackPositions[1] = centerPosition + Vector3.left * _offSetX;
        _attackPositions[2] = centerPosition + Vector3.right * _offSetX;
        _attackPositions[3] = centerPosition + Vector3.left * (_offSetX * 2f);
        _attackPositions[4] = centerPosition + Vector3.right * (_offSetX * 2f);

        ActivateWarning(0);

        yield return new WaitForSeconds(_warningInterval);

        ActivateWarning(1);
        ActivateWarning(2);

        yield return new WaitForSeconds(_warningInterval);

        ActivateWarning(3);
        ActivateWarning(4);

        yield return new WaitForSeconds(_waitAfterWarnings);

        _hasCast = true;

        ActivateIcicle(0);

        yield return new WaitForSeconds(_attackInterval);

        ActivateIcicle(1);
        ActivateIcicle(2);

        yield return new WaitForSeconds(_attackInterval);

        ActivateIcicle(3);
        ActivateIcicle(4);

        _sequence = null;
        Finish();
    }

    private void ActivateWarning(int index)
    {
        if (_telegraphs[index] == null) return;

        _telegraphs[index].Activate(_attackPositions[index], _telegraphScaleMultiplier);
    }

    private void ActivateIcicle(int index)
    {
        if (_icicles[index] != null)
        {
            Vector3 spawnPosition = _attackPositions[index] + Vector3.up * _offSetY;

            _icicles[index].Activate(spawnPosition);
            _icicles[index] = null;
        }

        if (_telegraphs[index] != null)
        {
            _telegraphs[index].Release();
            _telegraphs[index] = null;
        }
    }

    public override void Cast()
    {
        // 공격은 AttackSequence에서 실행
    }

    public override void Finish()
    {
        if (!_isRunning) return;
        if (_sequence != null) return;

        Cancel();
        _controller.OnActionFinished();
    }

    public override void Cancel()
    {
        if (_sequence != null)
        {
            StopCoroutine(_sequence);
            _sequence = null;
        }

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
            _telegraphs[i].Release();
            _telegraphs[i] = null;
        }
    }

    private void OnDisable()
    {
        Cancel();
    }
}