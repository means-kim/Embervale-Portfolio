using UnityEngine;
using UnityEngine.Video;
public class OutGameUI : MonoBehaviour
{
    [SerializeField] private string _gameSceneName = "MainTestScene";
    [SerializeField] private VideoClip _introClip;
    public void OnClickGameStart() => GameManager.LoadGameScene(_gameSceneName, introClip: _introClip, replayIntro: true);
    public void OnClickNewGame() => GameManager.LoadGameScene(_gameSceneName, true, _introClip);
    public void OnClickLeave()
    {
#if UNITY_EDITOR
        Debug.Log("[OutGameUI] LeaveButton 클릭: 에디터에서는 종료하지 않습니다.");
#else
        Application.Quit();
#endif
    }
}
