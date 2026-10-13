using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public sealed class EndingAchievementVideo : MonoBehaviour
{
    [SerializeField] private VideoClip _clip;
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private RawImage _image;
    private RenderTexture _texture;
    private bool _preparing;
    private bool _finished;
    private bool _failed;
    private bool _firstFrameReady;
    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        if (_image != null) _image.enabled = false;
    }

    public void Prepare()
    {
        if (_preparing || _player == null || _clip == null || _image == null) return;
        _preparing = true;
        _failed = _finished = _firstFrameReady = false;
        _texture = new RenderTexture((int)_clip.width, (int)_clip.height, 0);
        _texture.Create();
        _image.texture = _texture;
        _image.enabled = false;
        _player.playOnAwake = false;
        _player.source = VideoSource.VideoClip;
        _player.clip = _clip;
        _player.renderMode = VideoRenderMode.RenderTexture;
        _player.targetTexture = _texture;
        _player.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
        _player.audioOutputMode = VideoAudioOutputMode.Direct;
        _player.isLooping = false;
        _player.waitForFirstFrame = true;
        _player.sendFrameReadyEvents = true;
        _player.frameReady += HandleFrameReady;
        _player.loopPointReached += HandleFinished;
        _player.errorReceived += HandleError;
        _player.Prepare();
    }

    public IEnumerator PlayRoutine()
    {
        Prepare();
        if (_player == null || _clip == null || _image == null)
        {
            yield break;
        }

        float deadline = Time.unscaledTime + 10f;
        while (!_player.isPrepared && !_failed && Time.unscaledTime < deadline)
        {
            yield return null;
        }

        if (!_player.isPrepared || _failed)
        {
            StopPlayback();
            yield break;
        }

        IsPlaying = true;
        _player.Play();
        deadline = Time.unscaledTime + (float)_clip.length + 3f;
        while (!_finished && !_failed && Time.unscaledTime < deadline)
        {
            _image.enabled = _firstFrameReady;
            yield return null;
        }

        StopPlayback();
    }

    private void HandleFrameReady(VideoPlayer player, long frame) => _firstFrameReady = true;
    private void HandleFinished(VideoPlayer player) => _finished = true;

    private void HandleError(VideoPlayer player, string message)
    {
        _failed = true;
        Debug.LogWarning("Ending achievement video: " + message, this);
    }

    public void StopPlayback()
    {
        IsPlaying = false;
        _preparing = false;
        if (_image != null)
        {
            _image.enabled = false;
            _image.texture = null;
        }

        if (_player != null)
        {
            _player.frameReady -= HandleFrameReady;
            _player.loopPointReached -= HandleFinished;
            _player.errorReceived -= HandleError;
            _player.Stop();
            _player.targetTexture = null;
        }

        if (_texture != null)
        {
            _texture.Release();
            Destroy(_texture);
            _texture = null;
        }
    }

    private void OnDisable() => StopPlayback();
}