using UnityEngine;

public class AirBossAction : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Execute(AirBossActionType actionType)
    {
        switch (actionType)
        {
            case AirBossActionType.BothArmsLiftDrop:
                _animator.CrossFade("BothArmsLiftDrop", 0.1f);
                break;

            case AirBossActionType.ForwardReach:
                _animator.CrossFade("ForwardReach", 0.1f);
                break;

            case AirBossActionType.OneArmUpDown:
                _animator.CrossFade("OneArmUpDown", 0.1f);
                break;

            case AirBossActionType.BothArmsRaise:
                _animator.CrossFade("BothArmsRaise", 0.1f);
                break;

            case AirBossActionType.RaiseDropShadow:
                _animator.CrossFade("RaiseDropShadow", 0.1f);
                break;

            case AirBossActionType.SpinRoll360:
                _animator.CrossFade("SpinRoll360", 0.1f);
                break;

            case AirBossActionType.SpinYaw360:
                _animator.CrossFade("SpinYaw360", 0.1f);
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