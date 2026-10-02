using UnityEngine;
using UnityEngine.SceneManagement;
public class OutGameUI : MonoBehaviour
{
    [SerializeField] private string _gameSceneName = "YuchanScene";
    public void OnClickGameStart() => SceneManager.LoadScene(_gameSceneName);
}
