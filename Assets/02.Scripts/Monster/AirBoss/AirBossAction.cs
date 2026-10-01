using UnityEngine;

public class AirBossAction : MonoBehaviour
{
    public void Execute(AirBossActionType actionType)
    {
        switch (actionType)
        {
            case AirBossActionType.BothArmsLiftDrop:
                break;

            case AirBossActionType.ForwardReach:
                break;

            case AirBossActionType.OneArmUpDown:
                break;

            case AirBossActionType.BothArmsRaise:
                break;

            case AirBossActionType.RaiseDropShadow:
                break;

            case AirBossActionType.SpinRoll360:
                break;

            case AirBossActionType.SpinYaw360:
                break;
        }
    }
}