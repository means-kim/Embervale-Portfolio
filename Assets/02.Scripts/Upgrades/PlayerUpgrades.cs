using System;
using UnityEngine;

namespace Game.Upgrades
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-200)]
    public sealed class PlayerUpgrades : MonoBehaviour
    {
        [Serializable]
        private sealed class Rule
        {
            [Range(1, UpgradeHistory.MaxUpgradeLevel)] public int maxLevel = UpgradeHistory.MaxUpgradeLevel;
            [Min(0)] public int level;
        }

        [Header("단계와 최대 단계")]
        [SerializeField] private Rule _health = new Rule();
        [SerializeField] private Rule _damage = new Rule();
        [SerializeField] private Rule _speed = new Rule();

        [Header("단계당 증가량 (임시 기본값, Inspector에서 조정)")]
        [SerializeField, Min(0)] private int _healthPerLevel = 100;
        [SerializeField, Min(0)] private int _damagePerLevel = 3;
        [SerializeField, Min(0f)] private float _attackSpeedPerLevel = 0.1f;

        public event Action Changed;

        private void Awake() => LoadProgress();

        public void LoadProgress()
        {
            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                GetRule(type).level = Mathf.Clamp(UpgradeHistory.GetLevel(type), 0, GetMaxLevel(type));
            }
            Changed?.Invoke();
        }

        public void SaveProgress()
        {
            UpgradeHistory.SaveLevels(GetLevel(UpgradeType.MaxHealth), GetLevel(UpgradeType.AttackDamage), GetLevel(UpgradeType.AttackSpeed));
        }

        public int BonusMaxHealth => GetLevel(UpgradeType.MaxHealth) * Mathf.Max(0, _healthPerLevel);
        public int BonusAttackDamage => GetLevel(UpgradeType.AttackDamage) * Mathf.Max(0, _damagePerLevel);
        public float AttackSpeedMultiplier =>
            1f + GetLevel(UpgradeType.AttackSpeed) * Mathf.Max(0f, _attackSpeedPerLevel);

        public int GetLevel(UpgradeType type)
        {
            Rule rule = GetRule(type);
            return rule == null ? 0 : Mathf.Clamp(rule.level, 0, GetMaxLevel(type));
        }

        public int GetMaxLevel(UpgradeType type)
        {
            Rule rule = GetRule(type);
            return rule == null ? 0 : Mathf.Clamp(rule.maxLevel, 1, UpgradeHistory.MaxUpgradeLevel);
        }

        public bool CanUpgrade(UpgradeType type) => GetRule(type) != null && GetLevel(type) < GetMaxLevel(type);

        public bool TryUpgrade(UpgradeType type)
        {
            if (!CanUpgrade(type))
            {
                return false;
            }

            Rule rule = GetRule(type);
            rule.level = GetLevel(type) + 1;
            SaveProgress();
            Changed?.Invoke();
            return true;
        }

        public int GetMaxHealth(int baseMaxHealth)
        {
            return Mathf.Max(1, baseMaxHealth + BonusMaxHealth);
        }
        public int GetAttackDamage(int baseDamage)
        {
            return Mathf.Max(0, baseDamage + BonusAttackDamage);
        }
        public float GetAttackDuration(float baseDuration)
        {
            return Mathf.Max(0f, baseDuration) / AttackSpeedMultiplier;
        }

        public void ResetUpgrades()
        {
            _health.level = 0;
            _damage.level = 0;
            _speed.level = 0;
            UpgradeHistory.ResetProgress();
            Changed?.Invoke();
        }

        private Rule GetRule(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.MaxHealth:
                    return _health;
                case UpgradeType.AttackDamage:
                    return _damage;
                case UpgradeType.AttackSpeed:
                    return _speed;
                default: return null;
            }
        }

        private void OnValidate()
        {
            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                Rule rule = GetRule(type);
                if (rule == null)
                {
                    continue;
                }

                rule.maxLevel = Mathf.Clamp(rule.maxLevel, 1, UpgradeHistory.MaxUpgradeLevel);
                rule.level = Mathf.Clamp(rule.level, 0, rule.maxLevel);
            }
        }
    }
}
