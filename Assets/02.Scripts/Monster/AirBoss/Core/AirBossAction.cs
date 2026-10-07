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
                _animation.PlayForwardReachAnimation();
                break;

            case AirBossActionType.OneArmUpDown:
                StartAttack(_oneArmUpDown);
                break;

            case AirBossActionType.BothArmsRaise:
                _animation.PlayBothArmsRaiseAnimation();
                break;

            case AirBossActionType.RaiseDropShadow:
                _animation.PlayRaiseDropShadowAnimation();
                break;

            case AirBossActionType.SpinRoll360:
                _animation.PlaySpinRoll360Animation();
                break;

            case AirBossActionType.SpinYaw360:
                _animation.PlaySpinYaw360Animation();
                break;

            case AirBossActionType.BothArmsLightningGather:
                _animation.PlayBothArmsLightningGatherAnimation();
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
                _animation.PlayCrossBurstAnimation();
                break;
        }
    }

    public void ExecuteRandom()
    {
        // 작은 번개 패턴만 테스트
        Execute(AirBossActionType.BothArmsLiftDrop);

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

    private void CancelCurrentAttack()
    {
        if (_currentAttack == null) return;

        AirBossAttackBase canceled = _currentAttack;
        _currentAttack = null;
        canceled.Cancel();
    }
}