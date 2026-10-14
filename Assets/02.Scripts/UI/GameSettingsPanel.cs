using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public sealed class GameSettingsPanel : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider _sfxSlider;
    [SerializeField] private UnityEngine.UI.Slider _musicSlider;
    [SerializeField] private TMP_Text _sfxValue;
    [SerializeField] private TMP_Text _musicValue;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private TMP_Dropdown _glideDropdown;
    [SerializeField] private TMP_Dropdown _glideModeDropdown;

    private void OnEnable()
    {
        _sfxSlider.onValueChanged.AddListener(ChangeSfx);
        _musicSlider.onValueChanged.AddListener(ChangeMusic);
        _languageDropdown.onValueChanged.AddListener(ChangeLanguage);
        _glideDropdown.onValueChanged.AddListener(ChangeGlide);
        if (_glideModeDropdown != null) _glideModeDropdown.onValueChanged.AddListener(ChangeGlideMode);
        GameSettings.AudioChanged += RefreshVolumes;
        LocalizationSettings.SelectedLocaleChanged += RefreshLanguage;
        RefreshVolumes();
        StartCoroutine(InitializeLanguage());
    }

    private IEnumerator InitializeLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;
        RefreshLanguage(LocalizationSettings.SelectedLocale);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _sfxSlider.onValueChanged.RemoveListener(ChangeSfx);
        _musicSlider.onValueChanged.RemoveListener(ChangeMusic);
        _languageDropdown.onValueChanged.RemoveListener(ChangeLanguage);
        _glideDropdown.onValueChanged.RemoveListener(ChangeGlide);
        if (_glideModeDropdown != null) _glideModeDropdown.onValueChanged.RemoveListener(ChangeGlideMode);
        GameSettings.AudioChanged -= RefreshVolumes;
        LocalizationSettings.SelectedLocaleChanged -= RefreshLanguage;
        _languageDropdown.Hide();
        _glideDropdown.Hide();
        if (_glideModeDropdown != null) _glideModeDropdown.Hide();
        GameSettings.Save();
    }

    private void RefreshVolumes()
    {
        _sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume);
        _musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume);
        _sfxValue.text = Mathf.RoundToInt(GameSettings.SfxVolume * 100f) + "%";
        _musicValue.text = Mathf.RoundToInt(GameSettings.MusicVolume * 100f) + "%";
    }

    private void RefreshLanguage(Locale locale)
    {
        _languageDropdown.ClearOptions();
        _languageDropdown.AddOptions(new List<string> { "한국어", "English" });
        _languageDropdown.SetValueWithoutNotify(locale != null && locale.Identifier.Code == "en" ? 1 : 0);
        _glideDropdown.ClearOptions();
        _glideDropdown.AddOptions(new List<string>
        {
            LocalizationSettings.StringDatabase.GetLocalizedString("Menus", "glide.f"),
            LocalizationSettings.StringDatabase.GetLocalizedString("Menus", "glide.space")
        });
        _glideDropdown.SetValueWithoutNotify((int)GameSettings.GlideBinding);
        if (_glideModeDropdown != null)
        {
            _glideModeDropdown.ClearOptions();
            _glideModeDropdown.AddOptions(new List<string>
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("Menus", "glide.mode.toggle"),
                LocalizationSettings.StringDatabase.GetLocalizedString("Menus", "glide.mode.hold")
            });
            _glideModeDropdown.SetValueWithoutNotify((int)GameSettings.GlideMode);
        }
    }

    private void ChangeSfx(float value) => GameSettings.SetSfxVolume(value);
    private void ChangeMusic(float value) => GameSettings.SetMusicVolume(value);
    private void ChangeLanguage(int value) => GameSettings.SetLanguage(value == 1 ? "en" : "ko");
    private void ChangeGlide(int value) => GameSettings.SetGlideBinding(value);
    private void ChangeGlideMode(int value) => GameSettings.SetGlideMode(value);
}
