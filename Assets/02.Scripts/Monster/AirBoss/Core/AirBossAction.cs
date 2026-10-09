using UnityEngine;

public class AirBossAction : MonoBehaviour
{
    [SerializeField] private AirBossOneArmUpDownIce _oneArmUpDownIce;
    [SerializeField] private AirBossOneArmUpDownIceBlock _oneArmUpDownIceBlock;
    [SerializeField] private AirBossOneArmIceSummon _oneArmIceSummon;
    [SerializeField] private AirBossBothArmsIceThrow _bothArmsIceThrow;
    [SerializeField] private AirBossBothArmsIcePush _bothArmsIcePush;
    [SerializeField] private AirBossBothArmsLiftDrop _bothArmsLiftDrop;
    [SerializeField] private AirBossOneArmUpDown _oneArmUpDown;
    [SerializeField] private AirBossBothArmsRaise _bothArmsRaise;
    [SerializeField] private AirBossBothArmsLightningGather _bothArmsLightningGather;
    [SerializeField] private AirBossCrossBurst _crossBurst;
    [SerializeField] private AirBossForwardReach _forwardReach;
    [SerializeField] private AirBossCrossBurst6Point _crossBurst6Point;
    [SerializeField] private AirBossRaiseDropShadow _raiseDropShadow;
    [SerializeField] private AirBossOneArmUpDownIcePhase3 _oneArmUpDownIcePhase3;
    [SerializeField] private AirBossRaiseBothArmPhase3 _raiseBothArmPhase3;

    [Header("페이즈별 공격 패턴")]
    [SerializeField] private AirBossActionType[] _phase1Actions;

    [SerializeField] private AirBossActionType[] _phase2Actions;
    [SerializeField] private AirBossActionType[] _phase3Actions;


    private AirBossAnimation _animation;
    private AirBossAttackBase _currentAttack;

    private void Awake()
    {
        _animation = GetComponent<AirBossAnimation>();
    }

    public bool Execute(AirBossActionType actionType)
    {
        switch (actionType)
        {
            case AirBossActionType.BothArmsLiftDrop:
                return StartAttack(_bothArmsLiftDrop);

            case AirBossActionType.ForwardReach:
                return StartAttack(_forwardReach);

            case AirBossActionType.OneArmUpDown:
                return StartAttack(_oneArmUpDown);

            case AirBossActionType.BothArmsRaise:
                return StartAttack(_bothArmsRaise);

            case AirBossActionType.RaiseDropShadow:
                return StartAttack(_raiseDropShadow);

            // case AirBossActionType.SpinRoll360:
            //     _animation.PlaySpinRoll360Animation();
            //     break;
            //
            // case AirBossActionType.SpinYaw360:
            //     _animation.PlaySpinYaw360Animation();
            //     break;

            case AirBossActionType.BothArmsLightningGather:
                return StartAttack(_bothArmsLightningGather);

            case AirBossActionType.BothArmsIcePush:
                return StartAttack(_bothArmsIcePush);

            case AirBossActionType.BothArmsIceThrow:
                return StartAttack(_bothArmsIceThrow);

            case AirBossActionType.OneArmIceSummon:
                return StartAttack(_oneArmIceSummon);

            case AirBossActionType.OneArmUpDownIce:
                return StartAttack(_oneArmUpDownIce);

            case AirBossActionType.OneArmUpDownIceBlock:
                return StartAttack(_oneArmUpDownIceBlock);

            case AirBossActionType.CrossBurst:
                return StartAttack(_crossBurst);

            case AirBossActionType.CrossBurst6Point:
                return StartAttack(_crossBurst6Point);

            case AirBossActionType.OneArmUpDownIcePhase3:
                return StartAttack(_oneArmUpDownIcePhase3);

            case AirBossActionType.RaiseBothArmPhase3:
                return StartAttack(_raiseBothArmPhase3);

            default:
                Debug.LogWarning($"실행할 수 없는 공격입니다: {actionType}", this);
                return false;
        }
    }

    public bool ExecuteRandom(int phase)
    {
        // OnePatternTest();

        AirBossActionType[] actions;

        switch (phase)
        {
            case 1:
                actions = _phase1Actions;
                break;

            case 2:
                actions = _phase2Actions;
                break;

            case 3:
                actions = _phase3Actions;
                break;

            default:
                Debug.LogWarning($"잘못된 페이즈입니다: {phase}", this);
                return false;
        }

        if (actions == null || actions.Length == 0)
        {
            Debug.LogWarning($"페이즈 {phase}의 공격 목록이 비어있습니다.", this);
            return false;
        }

        int randomIndex = Random.Range(0, actions.Length);
        return Execute(actions[randomIndex]);
    }

    private bool StartAttack(AirBossAttackBase attack)
    {
        if (attack == null)
        {
            Debug.LogWarning("공격 패턴 컴포넌트가 연결되지 않았습니다.", this);
            return false;
        }

        CancelCurrentAttack();
        _currentAttack = attack;
        attack.BeginAttack();

        return _currentAttack == attack;
    }

    private void WarningCue()
    {
        if (_currentAttack != null)
        {
            _currentAttack.ShowWarning();
        }
    }

    private void CastCue()
    {
        if (_currentAttack != null)
        {
            _currentAttack.Cast();
        }
    }

    public void ActionFinished()
    {
        if (_currentAttack == null) return;

        _currentAttack.Finish();
    }

    public void CancelCurrentAttack()
    {
        if (_currentAttack == null) return;

        AirBossAttackBase canceled = _currentAttack;
        _currentAttack = null;
        canceled.Cancel();
    }

    private void OnePatternTest()
    {
        // 작은 번개 패턴만 테스트
        // Execute(AirBossActionType.BothArmsLiftDrop);

        // 큰 얼음 기둥 패턴만 테스트
        // Execute(AirBossActionType.BothArmsIceThrow);

        // 고드름 3발 패턴 테스트
        // Execute(AirBossActionType.OneArmUpDownIce);

        // 작은 얼음 기둥 패턴만 테스트
        // Execute(AirBossActionType.OneArmIceSummon);

        // 얼음 덩어리 패턴만 테스트
        // Execute(AirBossActionType.BothArmsIcePush);

        // 베지어 곡선 얼음 덩어리 패턴만 테스트
        // Execute(AirBossActionType.OneArmUpDownIceBlock);

        // 짧은 번개 패턴만 테스트
        // Execute(AirBossActionType.OneArmUpDown);

        // 큰 번개(보스 위치) 패턴만 테스트
        // Execute(AirBossActionType.BothArmsRaise);

        // 3줄 번개(보스 위치 센터) 패턴만 테스트
        // Execute(AirBossActionType.BothArmsLightningGather);

        // * 모양 번개 패턴만 테스트
        // Execute(AirBossActionType.CrossBurst);

        // // 번개 구체 패턴 테스트
        // Execute(AirBossActionType.ForwardReach);

        // 6줄기 번개 패턴만 테스트
        // Execute(AirBossActionType.CrossBurst6Point);

        // 눈보라 패턴만 테스트
        // Execute(AirBossActionType.RaiseDropShadow);

        // 순차적 고드름 5 패턴만 테스트
        // Execute(AirBossActionType.OneArmUpDownIcePhase3);

        // 순차적 번개 5 패턴만 테스트
        // Execute(AirBossActionType.RaiseBothArmPhase3);
    }
}