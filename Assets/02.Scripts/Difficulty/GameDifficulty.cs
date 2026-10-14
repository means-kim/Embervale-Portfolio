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
    public static Difficulty Current => _current ??= Validate((Difficulty)PlayerPrefs.GetInt(SaveKey, 0));
    public static float RecoveryMultiplier => Current == Difficulty.Easy ? 1.5f : 1f;
    public static float AttackMultiplier => Current == Difficulty.Easy ? 1.5f : 1f;
    public static float DamageTakenMultiplier => Current == Difficulty.Easy ? 0.5f : 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _current = null;

    public static void Set(Difficulty difficulty)
    {
        _current = Validate(difficulty);
        PlayerPrefs.SetInt(SaveKey, (int)Current);
        PlayerPrefs.Save();
    }

    private static Difficulty Validate(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => Difficulty.Easy,
        Difficulty.Hard => Difficulty.Hard,
        _ => Difficulty.Normal
    };
}
