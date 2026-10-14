using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UI_PlayerDeath : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Button _retryButton;
    [SerializeField, Min(0f)] private float _fadeDuration = 0.4f;
    [SerializeField] private AudioSource _deathAudioSource;

    private global::Health _health;
    private PlayerInput _input;
    private CanvasGroup _group;
    private Coroutine _fade;
    private bool _waitingForRetry;
    private bool _ready;
    private bool _restoreInput;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        if (_player == null || _retryButton == null)
        {
            enabled = false;
            return;
        }

        _health = _player.GetComponent<global::Health>();
        _input = _player.GetComponent<PlayerInput>();

        foreach (CanvasGroup child in GetComponentsInChildren<CanvasGroup>(true))
        {
            if (child == _group) continue;
            child.alpha = 1f;
            child.interactable = true;
            child.blocksRaycasts = child.gameObject == _retryButton.gameObject;
        }

        Hide();
    }

    private void OnEnable()
    {
        if (_health == null || _retryButton == null)
        {
            return;
        }

        _health.OnDie += ShowDeath;
        _player.OnRestarted += HandleRestarted;
        _retryButton.onClick.AddListener(Retry);
        if (_health.IsDead)
        {
            ShowDeath();
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDie -= ShowDeath;
        }

        if (_player != null)
        {
            _player.OnRestarted -= HandleRestarted;
        }

        if (_retryButton != null)
        {
            _retryButton.onClick.RemoveListener(Retry);
        }

        if (_fade != null) StopCoroutine(_fade);
        _fade = null;
        RestoreInput();
        if (_group != null && _retryButton != null)
        {
            Hide();
        }
    }

    private void ShowDeath()
    {
        if (_waitingForRetry)
        {
            return;
        }

        _waitingForRetry = true;
        _ready = false;
        _restoreInput = _input != null && _input.enabled;
        if (_input != null)
        {
            _input.enabled = false;
        }

        _group.interactable = false;
        _group.blocksRaycasts = true;
        _retryButton.interactable = false;
        _fade = StartCoroutine(FadeIn());
        PlayDeathAudio();
    }

    private void PlayDeathAudio()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeathBgm();
        }
        else if (_deathAudioSource != null)
        {
            _deathAudioSource.Play();
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _group.alpha = Mathf.Clamp01(elapsed / _fadeDuration);
            yield return null;
        }

        _group.alpha = 1f;
        _group.interactable = true;
        _retryButton.interactable = true;
        _ready = true;
        _fade = null;
    }

    public void Retry()
    {
        if (!_waitingForRetry || !_ready)
        {
            return;
        }

        _ready = false;
        _retryButton.interactable = false;
        if (!GameManager.RetryScene(gameObject.scene))
        {
            _ready = true;
            _retryButton.interactable = true;
        }
    }

    private void HandleRestarted()
    {
        AudioManager.Instance?.StopDeathBgm();
        if (AudioManager.Instance == null && _deathAudioSource != null) _deathAudioSource.Stop();
        if (_fade != null)
        {
            StopCoroutine(_fade);
        }

        _fade = null;
        RestoreInput();
        Hide();
    }

    private void RestoreInput()
    {
        if (_restoreInput && _input != null)
        {
            _input.enabled = true;
        }

        _restoreInput = false;
    }

    private void Hide()
    {
        _waitingForRetry = false;
        _ready = false;
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;
        _retryButton.interactable = false;
    }
}