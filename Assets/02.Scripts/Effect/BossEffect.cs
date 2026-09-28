using UnityEngine;

public class BossEffect : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartEffect(bool flipX)
    {
        if (flipX)
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
    }

    public void StopEffect()
    {
        Destroy(gameObject);
    }
}