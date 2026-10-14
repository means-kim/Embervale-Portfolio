using System;
using UnityEngine;

public abstract class CinematicSequence : MonoBehaviour
{
    public event Action Completed;
    public event Action Cancelled;

    public abstract bool TryPlay();
    public virtual bool TryPlayWithoutCinematic()
    {
        NotifyCompleted();
        return true;
    }

    public abstract void Cancel();
    public abstract void Skip();

    protected void EnableSkipping(CinematicManager manager)
    {
        CinematicSkipController.Begin(this, Skip, manager != null ? manager.SkipHoldDuration : 1.5f);
    }

    protected void NotifyCompleted()
    {
        CinematicSkipController.End(this);
        Completed?.Invoke();
    }

    protected void NotifyCancelled()
    {
        CinematicSkipController.End(this);
        Cancelled?.Invoke();
    }
}
