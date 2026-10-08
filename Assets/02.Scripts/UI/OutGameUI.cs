using UnityEngine;
using UnityEngine.Video;
public class OutGameUI : MonoBehaviour
{
    [SerializeField] private string _gameSceneName = "YuchanScene";
    [SerializeField] private VideoClip _introClip;
    public void OnClickGameStart() => GameManager.LoadGameScene(_gameSceneName, introClip: _introClip);
    public void OnClickNewGame() => GameManager.LoadGameScene(_gameSceneName, true, _introClip);
}
