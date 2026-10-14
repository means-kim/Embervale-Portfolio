using UnityEngine;

public enum Difficulty
{
    Normal = 0,
    Easy = 1,
    Hard = 2
}

public static class GameDifficulty
{
    private const string SaveKey = "Game.Difficulty";
    private static Difficulty? _current;
    public static Difficulty Current
    {
        get
        {
            if (_current.HasValue)
            {
                return _current.Value;
            }

            int saved = PlayerPrefs.GetInt(SaveKey, (int)Difficulty.Normal);
            if (saved == (int)Difficulty.Easy)
            {
                _current = Difficulty.Easy;
            }
            else if (saved == (int)Difficulty.Hard)
            {
                _current = Difficulty.Hard;
            }
            else
            {
                _current = Difficulty.Normal;
            }

            return _current.Value;
        }
    }
    public static float RecoveryMultiplier
    {
        get
        {
            if (Current == Difficulty.Easy)
            {
                return 1.5f;
            }

            if (Current == Difficulty.Hard)
            {
                return 0.5f;
            }

            return 1f;
        }
    }
    public static float AttackMultiplier
    {
        get
        {
            if (Current == Difficulty.Easy)
            {
                return 1.5f;
            }

            if (Current == Difficulty.Hard)
            {
                return 0.5f;
            }

            return 1f;
        }
    }
    public static float DamageTakenMultiplier
    {
        get
        {
            if (Current == Difficulty.Easy)
            {
                return 0.5f;
            }

            if (Current == Difficulty.Hard)
            {
                return 1.5f;
            }

            return 1f;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _current = null;

    public static void Set(Difficulty difficulty)
    {
        _current = difficulty;
        PlayerPrefs.SetInt(SaveKey, (int)Current);
        PlayerPrefs.Save();
    }
}
