using Game.Upgrades;
using UnityEngine;

public sealed class UI_UpgradeBinding : MonoBehaviour
{
    [SerializeField] private UI_Ability _abilityUI;
    [SerializeField] private PlayerUpgrades _upgrades;

    private void Awake()
    {
        if (_abilityUI == null) _abilityUI = GetComponent<UI_Ability>();
    }

    public void Bind(UI_Ability abilityUI, PlayerUpgrades upgrades)
    {
        OnDisable();
        _abilityUI = abilityUI;
        _upgrades = upgrades;
        if (isActiveAndEnabled)
        {
            OnEnable();
        }
    }

    private void OnEnable()
    {
        if (_abilityUI == null || _upgrades == null)
        {
            return;
        }

        _abilityUI.AbilityConfirmed += HandleConfirmed;
        _upgrades.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (_abilityUI != null)
        {
            _abilityUI.AbilityConfirmed -= HandleConfirmed;
        }

        if (_upgrades != null)
        {
            _upgrades.Changed -= Refresh;
        }
    }

    private void HandleConfirmed(UI_Ability.AbilityType type)
    {
        UpgradeType upgrade;
        switch (type)
        {
            case UI_Ability.AbilityType.Health:
                upgrade = UpgradeType.MaxHealth;
                break;
            case UI_Ability.AbilityType.Attack:
                upgrade = UpgradeType.AttackDamage;
                break;
            case UI_Ability.AbilityType.AttackSpeed:
                upgrade = UpgradeType.AttackSpeed;
                break;
            default:
                return;
        }

        _upgrades.TryUpgrade(upgrade);
    }

    public void Refresh()
    {
        if (_abilityUI == null || _upgrades == null)
        {
            return;
        }
        Sync(UI_Ability.AbilityType.Health, UpgradeType.MaxHealth);
        Sync(UI_Ability.AbilityType.Attack, UpgradeType.AttackDamage);
        Sync(UI_Ability.AbilityType.AttackSpeed, UpgradeType.AttackSpeed);
    }

    private void Sync(UI_Ability.AbilityType uiType, UpgradeType type)
    {
        _abilityUI.SetProgress(uiType, _upgrades.GetLevel(type), _upgrades.GetMaxLevel(type));
    }
}