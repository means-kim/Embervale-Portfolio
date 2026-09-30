using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerHit : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _hitDuration = 0.35f;
    [SerializeField, Min(0.01f)] private float _flashInterval = 0.08f;
    [SerializeField] private Color _flashColor = new Color(1f, 0.25f, 0.25f, 1f);

    public bool IsHit => Time.time < _hitUntil;

    private Health _health;
    private PlayerController _controller;
    private PlayerDash _dash;
    private SpriteRenderer[] _renderers;
    private Color[] _originalColors;
    private float _hitUntil;
    private float _flashTimer;
    private bool _flashOn;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _controller = GetComponent<PlayerController>();
        _dash = GetComponent<PlayerDash>();
        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].color;
    }

    private void OnEnable()
    {
        _health.OnHit += HandleHit;
        _health.OnDie += HandleDie;
    }

    private void OnDisable()
    {
        _health.OnHit -= HandleHit;
        _health.OnDie -= HandleDie;
        RestoreColors();
    }

    private void HandleHit()
    {
        if (_health.IsDead) return;
        _hitUntil = Time.time + _hitDuration;
        _flashTimer = 0f;
        _flashOn = false;
        if (_dash != null) _dash.CancelDash();
        EffectManager.Instance?.PlayPlayerHit(transform.position);
    }

    private void HandleDie()
    {
        _hitUntil = 0f;
        RestoreColors();
        EffectManager.Instance?.PlayPlayerDeath(transform.position);
        if (_controller != null) _controller.ChangeState(PlayerState.Death);
    }

    private void Update()
    {
        if (!IsHit)
        {
            if (_flashOn) RestoreColors();
            return;
        }
        _flashTimer += Time.deltaTime;
        if (_flashTimer < _flashInterval) return;
        _flashTimer = 0f;
        _flashOn = !_flashOn;
        ApplyFlash(_flashOn);
    }

    private void ApplyFlash(bool on)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] == null) continue;
            _renderers[i].color = on ? _flashColor : _originalColors[i];
        }
    }

    private void RestoreColors()
    {
        _flashOn = false;
        ApplyFlash(false);
    }
}
