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

    protected void NotifyCompleted()
    {
        Completed?.Invoke();
    }

    protected void NotifyCancelled()
    {
        Cancelled?.Invoke();
    }
}