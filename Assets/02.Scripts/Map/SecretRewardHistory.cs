using System.Collections.Generic;
using UnityEngine;

public static class SecretRewardHistory
{
    private const string SaveKey = "Progress.SecretRewards";

    [System.Serializable]
    private sealed class SaveData
    {
        public List<string> claimed = new List<string>();
    }

    public static string GetId(Transform gate)
    {
        return gate.gameObject.scene.path + ":" + CheckpointHistory.GetPath(gate);
    }

    private static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return new SaveData();
        try
        {
            SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey));
            return data != null && data.claimed != null ? data : new SaveData();
        }
        catch (System.ArgumentException)
        {
            return new SaveData();
        }
    }

    public static bool HasClaimed(string id) => Load().claimed.Contains(id);

    public static void MarkClaimed(string id)
    {
        SaveData data = Load();
        if (data.claimed.Contains(id)) return;
        data.claimed.Add(id);
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}