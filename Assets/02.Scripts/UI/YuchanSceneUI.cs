using TMPro;
using UnityEngine;
public class YuchanSceneUI : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Health _playerHealth;
    [SerializeField] private PlayerLevel _playerLevel;
    [Header("버튼 설정")]
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private int _levelUpAmount = 1;
    private int _lastShownHealth = int.MinValue;
    private int _lastShownMaxHealth = int.MinValue;
    private int _lastShownLevel = int.MinValue;
    private void Start()
    {
        ResolvePlayerRefs();
        RefreshHpText(true);
        RefreshLevelText(true);
    }
    private void Update()
    {
        RefreshHpText(false);
        RefreshLevelText(false);
    }
    public void OnClickDamageButton()
    {
        if (_playerHealth == null) return;
        _playerHealth.TakeDamage(_damageAmount);
        RefreshHpText(true);
    }
    public void OnClickLevelUpButton()
    {
        if (_playerLevel == null) return;
        _playerLevel.AddLevel(_levelUpAmount);
        RefreshLevelText(true);
    }
    private void ResolvePlayerRefs()
    {
        if (_playerHealth != null && _playerLevel != null) return;
        var playerObject = GameObject.Find("Player");
        if (playerObject == null) return;
        if (_playerHealth == null) _playerHealth = playerObject.GetComponent<Health>();
        if (_playerLevel == null) _playerLevel = playerObject.GetComponent<PlayerLevel>();
    }
    private void RefreshHpText(bool force)
    {
        if (_hpText == null || _playerHealth == null) return;
        int current = _playerHealth.CurrentHealth;
        int max = _playerHealth.MaxHealth;
        if (!force && current == _lastShownHealth && max == _lastShownMaxHealth) return;
        _lastShownHealth = current;
        _lastShownMaxHealth = max;
        _hpText.text = $"{current} / {max}";
    }
    private void RefreshLevelText(bool force)
    {
        if (_levelText == null || _playerLevel == null) return;
        int level = _playerLevel.Level;
        if (!force && level == _lastShownLevel) return;
        _lastShownLevel = level;
        _levelText.text = $"Level {level}";
    }
}
