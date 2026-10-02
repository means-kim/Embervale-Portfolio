using UnityEngine;

public class AirBossBothArmsLiftDrop : AirBossAttackBase
{
    [Header("보스 컴포넌트")]
    [SerializeField] private AirBossAnimation _animation;

    [SerializeField] private AirBossController _controller;

    [Header("플레이어 위치")]
    [SerializeField] private Transform _player;

    [Header("텔레그래프 프리팹")]
    [SerializeField] private GameObject _telegraphPrefab;

    private AirBossAttack _lightnig;
    private Vector3 _telegraphPosition;

    private bool _isRunning = false;
    private bool _isWarnigShow = false;

    public override void BeginAttack()
    {
        Cancel();

        if (_controller == null)
        {
            Debug.LogError("보스 컨트롤러를 연결하세요.", this);
            return;
        }

        if (_animation == null || _player == null || _telegraphPrefab == null)
        {
            Debug.Log("공격에 필요한 참조를 연결하세요.", this);
            _controller.OnActionFinished();
            return;
        }

        AirBossAttackPool pool = AirBossAttackPool.Instance;

        if (pool == null)
        {
            _controller.OnActionFinished();
            return;
        }

        _lightnig = pool.GetAttack(AirBossAttackType.SmallLightning);

        if (_lightnig == null)
        {
            _controller.OnActionFinished();
            return;
        }

        _isRunning = true;
        _animation.PlayBothArmsLiftDropAnimation();
    }

    public override void ShowWarning()
    {
        if (!_isRunning) return;
        if (_isWarnigShow) return;

        if (_player == null)
        {
            Finish();
            return;
        }

        _telegraphPosition = _player.position;
        _telegraphPrefab.transform.position = _telegraphPosition;
        _telegraphPrefab.SetActive(true);

        _isWarnigShow = true;
    }

    public override void Cast()
    {
        if (!_isRunning) return;
        if (!_isWarnigShow) return;
        if (_lightnig == null) return;

        _lightnig.Activate(_telegraphPosition);

        _lightnig = null;
        _telegraphPrefab.SetActive(false);
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

        if (_telegraphPrefab != null)
        {
            _telegraphPrefab.SetActive(false);
        }

        if (_lightnig != null)
        {
            _lightnig.Release();
            _lightnig = null;
        }
    }

    private void OnDisable()
    {
        Cancel();
    }
}