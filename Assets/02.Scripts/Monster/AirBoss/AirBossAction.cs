using UnityEngine;

public class AirBossAction : MonoBehaviour
{
    private AirBossAnimation _animation;

    private void Awake()
    {
        _animation = GetComponent<AirBossAnimation>();
    }

    public void Execute(AirBossActionType actionType)
    {
        switch (actionType)
        {
            case AirBossActionType.BothArmsLiftDrop:
                _animation.PlayBothArmsLiftDropAnimation();
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
        }
    }

    public void ExecuteRandom()
    {
        int randomIndex = Random.Range(0, (int)AirBossActionType.Count);

        AirBossActionType actionType = (AirBossActionType)randomIndex;

        Execute(actionType);
    }
}