using UnityEngine;

public abstract class AirBossAttackBase : MonoBehaviour
{
    public abstract void BeginAttack();
    public abstract void ShowWarning();
    public abstract void Cast();

    public abstract void Finish();
    public abstract void Cancel();
}