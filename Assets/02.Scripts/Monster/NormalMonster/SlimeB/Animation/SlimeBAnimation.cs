using UnityEngine;

public class SlimeBAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayIdleAnimation()
    {
        _animator.CrossFade("SlimeIBdle", 0.1f);
    }

    public void PlayMoveAnimation()
    {
        _animator.CrossFade("SlimeBMove", 0.1f);
    }

    public void PlayHitAnimation()
    {
        _animator.CrossFade("SlimeBHit", 0.1f);
    }

    public void PlayDeathAnimation()
    {
        _animator.CrossFade("SlimeBDeath", 0.1f);
    }
}