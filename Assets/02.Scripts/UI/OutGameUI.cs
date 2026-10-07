using UnityEngine;
public class OutGameUI : MonoBehaviour
{
    [SerializeField] private string _gameSceneName = "YuchanScene";
    public void OnClickGameStart() => GameManager.LoadGameScene(_gameSceneName);
    public void OnClickNewGame() => GameManager.LoadGameScene(_gameSceneName, true);
}
