using UnityEngine;

public abstract class AirBossAttackBase : MonoBehaviour
{
    public uint AudioGeneration { get; private set; }
    public bool AudioActive { get; private set; }

    public void BeginAttack()
    {
        Cancel();
        AudioActive = true;
        BeginAttackCore();
    }

    protected abstract void BeginAttackCore();
    public abstract void ShowWarning();
    public abstract void Cast();

    public abstract void Finish();

    public void Cancel()
    {
        AudioActive = false;
        AudioGeneration++;
        AttackOneShotAudio.StopScope(this);
        CancelCore();
    }

    protected abstract void CancelCore();
}