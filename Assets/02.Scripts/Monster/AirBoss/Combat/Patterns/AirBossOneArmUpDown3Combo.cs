using UnityEngine;
using System.Collections;

public class AirBossOneArmUpDown3Combo : AirBossAttackBase
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

    [Header("경고 표시 후 짧은 번개 생성까지의 시간")]
    [SerializeField, Min(0f)] private float _waitAfterWarnings = 0.5f;

    [Header("짧은 번개 생성 후 다음 경고까지의 시간")]
    [SerializeField, Min(0f)] private float _attackInterval = 0.2f;

    private Coroutine _sequence;

    private const int AttackCount = 3;

    private AirBossAttack[] _shortLightning = new AirBossAttack[AttackCount];
    private Telegraph[] _telegraphs = new Telegraph[AttackCount];
    private Vector3[] _attackPositions = new Vector3[AttackCount];

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

        for (int i = 0; i < _shortLightning.Length; i++)
        {
            _shortLightning[i] = attackPool.GetAttack(AirBossAttackType.ShortLightning);
            _telegraphs[i] = telegraphPool.GetTelegraph(_telegraphType);

            if (_shortLightning[i] == null || _telegraphs[i] == null)
            {
                Cancel();
                _controller.OnActionFinished();
                return;
            }
        }

        _isRunning = true;
        _animation.PlayAirBossOneArmUpDown3ComboAnimation();
    }

    public override void ShowWarning()
    {
        if (!_isRunning) return;
        if (_isWarningShown) return;
        if (_hasCast) return;

        if (_player == null)
        {
            Finish();
            return;
        }

        _isWarningShown = true;
        _sequence = StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // StartCoroutine의 반환값이 _sequence에 저장된 후 진행합니다.
        yield return null;

        for (int i = 0; i < AttackCount; i++)
        {
            if (_player == null)
            {
                _sequence = null;
                Finish();
                yield break;
            }

            // 매회 경고 시점의 위치를 저장하고, 그 위치에 짧은 번개을 생성합니다.
            _attackPositions[i] = _player.position;
            ActivateWarning(i);

            yield return new WaitForSeconds(_waitAfterWarnings);

            ActivateIcicle(i);

            if (i < AttackCount - 1)
            {
                yield return new WaitForSeconds(_attackInterval);
            }
        }

        _hasCast = true;
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
        if (_shortLightning[index] != null)
        {
            _shortLightning[index].Activate(_attackPositions[index]);
            _shortLightning[index] = null;
        }

        if (_telegraphs[index] != null)
        {
            _telegraphs[index].FadeOutAndRelease();
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

        for (int i = 0; i < _shortLightning.Length; i++)
        {
            if (_shortLightning[i] == null) continue;

            _shortLightning[i].Release();
            _shortLightning[i] = null;
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