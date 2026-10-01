using UnityEngine;

public class AirBossAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
}