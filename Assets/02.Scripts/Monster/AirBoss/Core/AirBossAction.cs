using UnityEngine;

public class AirBossAction : MonoBehaviour
{
    [SerializeField] private AirBossBothArmsIceThrow _bothArmsIceThrow;
    [SerializeField] private AirBossBothArmsLiftDrop _bothArmsLiftDrop;

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
                _animation.PlayOneArmUpDownAnimation();
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
                _animation.PlayBothArmsLightningGather();
                break;

            case AirBossActionType.BothArmsIcePush:
                _animation.PlayBothArmsIcePush();
                break;

            case AirBossActionType.BothArmsIceThrow:
                StartAttack(_bothArmsIceThrow);
                break;

            case AirBossActionType.OneArmIceSummon:
                _animation.PlayOneArmIceSummon();
                break;

            case AirBossActionType.OneArmUpDownIce:
                _animation.PlayOneArmUpDownIce();
                break;
        }
    }

    public void ExecuteRandom()
    {
        // 작은 번개 패턴만 테스트
        // Execute(AirBossActionType.BothArmsLiftDrop);

        // 큰 얼음 기둥 패턴만 테스트
        Execute(AirBossActionType.BothArmsIceThrow);

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