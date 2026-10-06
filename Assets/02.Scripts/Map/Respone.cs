using UnityEngine;
using TestPlayer = HyunseoTest.Player;

public class Respone : MonoBehaviour
{
    // Respawn currently targets Player_Hyunseo only, before main-player integration.
    [SerializeField, Min(0)] private int _returnDamage = 100;
    private static Respone _instance = null;
    public static Respone Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void ResponePlayer(TestPlayer player, Transform checkpoint)
    {
        if (checkpoint == null) return;
        if (player == null) return;
        var point = checkpoint.GetComponent<RespawnPoint>();
        if (point != null) player.SetRespawnPoint(point);
        else player.SetRespawnPoint(checkpoint.position);
        ResponePlayer(player, checkpoint.position);
    }

    public void ResponePlayer(TestPlayer player, Vector3 checkpointPosition)
    {
        if (player == null) return;
        Health health = player.GetComponent<Health>();

        if (health.IsDead)
            return;

        health.TakeDamage(_returnDamage);
        player.MoveToCheckpoint(checkpointPosition);
    }

    public void RestartPlayer(TestPlayer player, Transform checkpoint)
    {
        if (player == null || checkpoint == null) return;
        var point = checkpoint.GetComponent<RespawnPoint>();
        if (point != null) player.SetRespawnPoint(point);
        else player.SetRespawnPoint(checkpoint.position);
        player.RestartAt(checkpoint.position);
    }

    // Parameterless entry point for the test player's retry button.
    public void RestartTestPlayer()
    {
        TestPlayer player = GetComponent<TestPlayer>();
        if (player != null) player.RestartAtCheckpoint();
    }
}
