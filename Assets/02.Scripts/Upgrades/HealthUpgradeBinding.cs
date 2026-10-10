using UnityEngine;
using Game.Upgrades;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(PlayerUpgrades))]
[DefaultExecutionOrder(-100)]
public sealed class HealthUpgradeBinding : MonoBehaviour
{
    private Health _health;
    private PlayerUpgrades _upgrades;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _upgrades = GetComponent<PlayerUpgrades>();
    }

    private void OnEnable()
    {
        _upgrades.Changed += Apply;
        Apply();
    }

    private void OnDisable()
    {
        if (_upgrades != null)
        {
            _upgrades.Changed -= Apply;
        }
    }

    private void Apply()
    {
        _health.SetMaxHealthBonus(_upgrades.BonusMaxHealth);
    }
}
