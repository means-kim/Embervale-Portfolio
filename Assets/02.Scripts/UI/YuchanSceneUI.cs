using TMPro;
using UnityEngine;

public class YuchanSceneUI : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Health _playerHealth;

    private int _lastShownHealth = int.MinValue;
    private int _lastShownMaxHealth = int.MinValue;

    private void Start()
    {
        if (_playerHealth == null)
        {
            var playerObject = GameObject.Find("Player");
            if (playerObject != null)
                _playerHealth = playerObject.GetComponent<Health>();
        }
        RefreshText(true);
    }

    private void Update()
    {
        RefreshText(false);
    }

    private void RefreshText(bool force)
    {
        if (_hpText == null || _playerHealth == null) return;
        int current = _playerHealth.CurrentHealth;
        int max = _playerHealth.MaxHealth;
        if (!force && current == _lastShownHealth && max == _lastShownMaxHealth) return;
        _lastShownHealth = current;
        _lastShownMaxHealth = max;
        _hpText.text = $"{current} / {max}";
    }
}
