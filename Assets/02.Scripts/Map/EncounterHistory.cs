using System.Collections.Generic;
using UnityEngine;

public enum EncounterId
{
    Elite01,
    Boss01
}

public enum EliminateId
{
    Elite01,
    Boss01
}
public class EncounterHistory : MonoBehaviour
{
    private static EncounterHistory _instance;
    public static EncounterHistory Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public bool HasSeenIntro(EncounterId id)
    {
        return PlayerPrefs.GetInt($"Encounter.IntroSeen.{id}", 0) == 1;
    }
    public bool IsEliminate(EliminateId id)
    {
        return HasEliminated(id);
    }

    public static bool HasEliminated(EliminateId id) => PlayerPrefs.GetInt($"EliminateId.{id}", 0) == 1;

    public void MarkIntroSeen(EncounterId id)
    {
        PlayerPrefs.SetInt($"Encounter.IntroSeen.{id}", 1);
        PlayerPrefs.Save();
    }
    public void MarkEliminate(EliminateId id)
    {
        PlayerPrefs.SetInt($"EliminateId.{id}", 1);
        PlayerPrefs.Save();
    }
    [ContextMenu("Reset Encounter Progress")]
    public void ResetProgress()
    {
        ResetSavedProgress();
    }

    public static void ResetSavedProgress()
    {
        foreach (EncounterId id in System.Enum.GetValues(typeof(EncounterId)))
        {
            PlayerPrefs.DeleteKey($"Encounter.IntroSeen.{id}");
        }

        foreach (EliminateId id in System.Enum.GetValues(typeof(EliminateId)))
        {
            PlayerPrefs.DeleteKey($"EliminateId.{id}");
        }

        PlayerPrefs.Save();
    }

}
