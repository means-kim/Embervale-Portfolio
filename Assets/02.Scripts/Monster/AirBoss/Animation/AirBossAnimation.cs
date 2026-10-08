using UnityEngine;

public class AirBossAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayEncounterAnimation()
    {
        _animator.CrossFade("Encounter", 0.1f);
    }

    public void PlayIdleAnimation()
    {
        _animator.CrossFade("Idle", 0.1f);
    }

    public void PlayMoveAnimation(Vector2 direction)
    {
        if (direction.x < 0)
        {
            _animator.CrossFade("MoveBankLeft", 0.1f);
        }
        else if (direction.x > 0)
        {
            _animator.CrossFade("MoveBankRight", 0.1f);
        }
        else
        {
            _animator.CrossFade("Idle", 0.1f);
        }
    }

    public void PlayBothArmsLiftDropAnimation()
    {
        _animator.CrossFade("BothArmsLiftDrop", 0.1f);
    }

    public void PlayForwardReachAnimation()
    {
        _animator.CrossFade("ForwardReach", 0.1f);
    }

    public void PlayOneArmUpDownAnimation()
    {
        _animator.CrossFade("OneArmUpDown", 0.1f);
    }

    public void PlayBothArmsRaiseAnimation()
    {
        _animator.CrossFade("BothArmsRaise", 0.1f);
    }

    public void PlayRaiseDropShadowAnimation()
    {
        _animator.CrossFade("RaiseDropShadow", 0.1f);
    }

    public void PlaySpinRoll360Animation()
    {
        _animator.CrossFade("SpinRoll360", 0.1f);
    }

    public void PlaySpinYaw360Animation()
    {
        _animator.CrossFade("SpinYaw360", 0.1f);
    }

    public void PlayBothArmsLightningGatherAnimation()
    {
        _animator.CrossFade("BothArmsLightningGather", 0.1f);
    }

    public void PlayBothArmsIceThrowAnimation()
    {
        _animator.CrossFade("BothArmsIceThrow", 0.1f);
    }

    public void PlayBothArmsIcePushAnimation()
    {
        _animator.CrossFade("BothArmsIcePush", 0.1f);
    }

    public void PlayOneArmIceSummonAnimation()
    {
        _animator.CrossFade("OneArmIceSummon", 0.1f);
    }

    public void PlayOneArmUpDownIceAnimation()
    {
        _animator.CrossFade("OneArmUpDownIce", 0.1f);
    }

    public void PlayOneArmUpDownIceBlockAnimation()
    {
        _animator.CrossFade("OneArmUpDownIceBlock", 0.1f);
    }

    public void PlayCrossBurstAnimation()
    {
        _animator.CrossFade("CrossBurst", 0.1f);
    }

    public void PlayCrossBurst6PointAnimation()
    {
        _animator.CrossFade("CrossBurst6Point", 0.1f);
    }

    public void PlayOneArmUpDownIcePhase3Animation()
    {
        _animator.CrossFade("OneArmUpDownIcePhase3", 0.1f);
    }

    public void PlayDeathAnimation()
    {
        _animator.CrossFade("Death", 0.1f);
    }
}