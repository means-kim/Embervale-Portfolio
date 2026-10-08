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

    private AirBossAnimation _animation;
    private AirBossAttackBase _currentAttack;

    private void Awake()
    {
        _animation = GetComponent<AirBossAnimation>();
    }

    public void Execute(AirBossActionType actionType)
    {
        switch (actionType)
        {
            case AirBossActionType.BothArmsLiftDrop:
                StartAttack(_bothArmsLiftDrop);
                break;

            case AirBossActionType.ForwardReach:
                StartAttack(_forwardReach);
                break;

            case AirBossActionType.OneArmUpDown:
                StartAttack(_oneArmUpDown);
                break;

            case AirBossActionType.BothArmsRaise:
                StartAttack(_bothArmsRaise);
                break;

            case AirBossActionType.RaiseDropShadow:
                StartAttack(_raiseDropShadow);
                break;

            case AirBossActionType.SpinRoll360:
                _animation.PlaySpinRoll360Animation();
                break;

            case AirBossActionType.SpinYaw360:
                _animation.PlaySpinYaw360Animation();
                break;

            case AirBossActionType.BothArmsLightningGather:
                StartAttack(_bothArmsLightningGather);
                break;

            case AirBossActionType.BothArmsIcePush:
                StartAttack(_bothArmsIcePush);
                break;

            case AirBossActionType.BothArmsIceThrow:
                StartAttack(_bothArmsIceThrow);
                break;

            case AirBossActionType.OneArmIceSummon:
                StartAttack(_oneArmIceSummon);
                break;

            case AirBossActionType.OneArmUpDownIce:
                StartAttack(_oneArmUpDownIce);
                break;

            case AirBossActionType.OneArmUpDownIceBlock:
                StartAttack(_oneArmUpDownIceBlock);
                break;

            case AirBossActionType.CrossBurst:
                StartAttack(_crossBurst);
                break;

            case AirBossActionType.CrossBurst6Point:
                StartAttack(_crossBurst6Point);
                break;

            case AirBossActionType.OneArmUpDownIcePhase3:
                StartAttack(_oneArmUpDownIcePhase3);
                break;
        }
    }

    public void ExecuteRandom()
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
        Execute(AirBossActionType.OneArmUpDownIceBlock);

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

        // int randomIndex = Random.Range(0, (int)AirBossActionType.Count);
        //
        // AirBossActionType actionType = (AirBossActionType)randomIndex;
        //
        // Execute(actionType);
    }

    private void StartAttack(AirBossAttackBase attack)
    {
        CancelCurrentAttack();

        if (attack == null) return;

        _currentAttack = attack;
        _currentAttack.BeginAttack();
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

        AirBossAttackBase finished = _currentAttack;
        _currentAttack = null;

        finished.Finish();
    }

    public void CancelCurrentAttack()
    {
        if (_currentAttack == null) return;

        AirBossAttackBase canceled = _currentAttack;
        _currentAttack = null;
        canceled.Cancel();
    }
}