using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public sealed class GameSettingsRuntime : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        var host = new GameObject("Game Settings");
        DontDestroyOnLoad(host);
        host.AddComponent<GameSettingsRuntime>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameSettings.LanguageRequested += ApplyLanguage;
        ApplyLanguage();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameSettings.LanguageRequested -= ApplyLanguage;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var source in root.GetComponentsInChildren<AudioSource>(true))
            {
                if (source.GetComponent<SettingsAudioSource>() == null) SettingsAudioSource.Bind(source);
            }
        }
    }

    private void ApplyLanguage()
    {
        StopAllCoroutines();
        StartCoroutine(SelectSavedLocale());
    }

    private IEnumerator SelectSavedLocale()
    {
        yield return LocalizationSettings.InitializationOperation;
        var locale = LocalizationSettings.AvailableLocales.GetLocale(GameSettings.SavedLanguage);
        if (locale != null) LocalizationSettings.SelectedLocale = locale;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) GameSettings.Save();
    }

    private void OnApplicationQuit() => GameSettings.Save();
}
