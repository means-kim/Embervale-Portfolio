using System;
using UnityEngine;

public enum GlideKeyBinding { F, Space }

public static class GameSettings
{
    public const string SfxVolumeKey = "Settings.SfxVolume";
    public const string MusicVolumeKey = "Settings.MusicVolume";
    public const string LanguageKey = "Settings.Language";
    public const string GlideKey = "Settings.GlideKey";

    public static event Action AudioChanged;
    public static event Action LanguageRequested;
    public static float SfxVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));
    public static float MusicVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
    public static string SavedLanguage => PlayerPrefs.GetString(LanguageKey, "ko");
    public static GlideKeyBinding GlideBinding => PlayerPrefs.GetInt(GlideKey, 0) == 1
        ? GlideKeyBinding.Space : GlideKeyBinding.F;

    public static void SetSfxVolume(float value)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
        AudioChanged?.Invoke();
    }

    public static void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
        AudioChanged?.Invoke();
    }

    public static void SetLanguage(string code)
    {
        if (code != "ko" && code != "en") return;
        PlayerPrefs.SetString(LanguageKey, code);
        LanguageRequested?.Invoke();
    }

    public static void SetGlideBinding(int value) => PlayerPrefs.SetInt(GlideKey, value == 1 ? 1 : 0);
    public static void Save() => PlayerPrefs.Save();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetEvents()
    {
        AudioChanged = null;
        LanguageRequested = null;
    }
}
