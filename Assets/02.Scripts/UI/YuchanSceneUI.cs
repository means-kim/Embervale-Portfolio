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

    // 버튼 OnClick 연결용: 플레이어에게 데미지를 준다.
    public void OnClickDamageButton()
    {
        if (_playerHealth == null) return;
        _playerHealth.TakeDamage(_damageAmount);
        RefreshHpText(true);
    }

    // 버튼 OnClick 연결용: 플레이어 레벨을 올린다.
    public void OnClickLevelUpButton()
    {
        if (_playerLevel == null) return;
        _playerLevel.AddLevel(_levelUpAmount);
        RefreshLevelText(true);
    }

    // 인스펙터 연결 우선, 비어 있을 때만 Player 오브젝트에서 찾는다.
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
        _levelText.text = $"{level}";
    }
}
