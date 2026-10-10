using UnityEngine;

public static class UpgradeHistory
{
    private static string Key(UpgradeType type)
    {
        return "Upgrade.Level." + type;
    }


    public static int GetLevel(UpgradeType type)
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(Key(type), 0));
    }

    public static void SaveLevels(int health, int damage, int speed)
    {
        PlayerPrefs.SetInt(Key(UpgradeType.MaxHealth), Mathf.Max(0, health));
        PlayerPrefs.SetInt(Key(UpgradeType.AttackDamage), Mathf.Max(0, damage));
        PlayerPrefs.SetInt(Key(UpgradeType.AttackSpeed), Mathf.Max(0, speed));
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
