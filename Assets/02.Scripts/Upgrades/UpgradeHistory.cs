using UnityEngine;

public static class UpgradeHistory
{
    public const int MaxUpgradeLevel = 5;

    private static string Key(UpgradeType type)
    {
        return "Upgrade.Level." + type;
    }


    public static int GetLevel(UpgradeType type)
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(Key(type), 0), 0, MaxUpgradeLevel);
    }

    public static void SaveLevels(int health, int damage, int speed)
    {
        PlayerPrefs.SetInt(Key(UpgradeType.MaxHealth), Mathf.Clamp(health, 0, MaxUpgradeLevel));
        PlayerPrefs.SetInt(Key(UpgradeType.AttackDamage), Mathf.Clamp(damage, 0, MaxUpgradeLevel));
        PlayerPrefs.SetInt(Key(UpgradeType.AttackSpeed), Mathf.Clamp(speed, 0, MaxUpgradeLevel));
        PlayerPrefs.Save();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(Key(UpgradeType.MaxHealth));
        PlayerPrefs.DeleteKey(Key(UpgradeType.AttackDamage));
        PlayerPrefs.DeleteKey(Key(UpgradeType.AttackSpeed));
        PlayerPrefs.Save();
    }
}
