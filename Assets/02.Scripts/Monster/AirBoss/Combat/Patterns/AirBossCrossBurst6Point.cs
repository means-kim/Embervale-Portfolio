using UnityEngine;
using System.Collections;

public class AirBossCrossBurst6Point : AirBossAttackBase
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

    [Header("코루틴에 사용")]
    [SerializeField] private float _warningInterval = 0.2f;

    [SerializeField] private float _waitAfterWarnings = 0.5f;
    [SerializeField] private float _attackInterval = 0.2f;

    private Coroutine _sequence;

    private const int AttackCount = 6;
    private readonly float[] _attackAngles = { 0f, 150f, 120f, 90f, 60f, 30f };

    private AirBossAttack[] _horizontalSmallLightnings = new AirBossAttack[AttackCount];
    private Telegraph[] _telegraphs = new Telegraph[AttackCount];
    private Vector3[] _attackPositions = new Vector3[AttackCount];

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

        for (int i = 0; i < _horizontalSmallLightnings.Length; i++)
        {
            _horizontalSmallLightnings[i] = attackPool.GetAttack(AirBossAttackType.SmallLightning, this);
            _telegraphs[i] = telegraphPool.GetTelegraph(_telegraphType, this);

            if (_horizontalSmallLightnings[i] == null || _telegraphs[i] == null)
            {
                Cancel();
                _controller.OnActionFinished();
                return;
            }
        }

        _isRunning = true;
        _animation.PlayCrossBurst6PointAnimation();
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

        // 중복 실행 방지
        _isWarningShown = true;

        Vector3 centerPosition = _player.position;

        _sequence = StartCoroutine(AttackSequence(centerPosition));
    }

    private IEnumerator AttackSequence(Vector3 centerPosition)
    {
        for (int i = 0; i < AttackCount; i++)
        {
            if (i > 0)
            {
                yield return new WaitForSeconds(_warningInterval);
            }

            // 공격 모두 같은 중심 사용
            _attackPositions[i] = centerPosition;

            // 텔레그래프 회전
            _telegraphs[i].transform.rotation = Quaternion.Euler(0f, 0f, _attackAngles[i]);

            _telegraphs[i].Activate(_attackPositions[i], _telegraphScaleMultiplier);
        }

        yield return new WaitForSeconds(_waitAfterWarnings);

        _hasCast = true;

        for (int i = 0; i < AttackCount; i++)
        {
            if (i > 0)
            {
                yield return new WaitForSeconds(_attackInterval);
            }

            AirBossAttack attack = _horizontalSmallLightnings[i];

            if (attack == null) continue;

            attack.transform.rotation = Quaternion.Euler(0f, 0f, _attackAngles[i]);

            attack.Activate(_attackPositions[i]);
            _horizontalSmallLightnings[i] = null;

            if (_telegraphs[i] != null)
            {
                _telegraphs[i].FadeOutAndRelease();
                _telegraphs[i] = null;
            }
        }

        _sequence = null;
        Finish();
    }

    public override void Cast()
    {
    }

    public override void Finish()
    {
        if (!_isRunning) return;
        if (_sequence != null) return;

        Cancel();
        _controller.OnActionFinished();
    }

    protected override void CancelCore()
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

        for (int i = 0; i < _horizontalSmallLightnings.Length; i++)
        {
            if (_horizontalSmallLightnings[i] == null) continue;

            _horizontalSmallLightnings[i].Release();
            _horizontalSmallLightnings[i] = null;
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