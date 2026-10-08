using UnityEngine;
using TestPlayer = HyunseoTest.Player;

public class Respone : MonoBehaviour
{
    // Shared respawn service. Can live on a separate scene object.
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

    // Compatibility overloads for existing HyunseoTest player scripts and scenes.
    public void ResponePlayer(TestPlayer player, Transform checkpoint)
    {
        if (checkpoint == null) return;
        if (player == null) return;
        ResponePlayer(player, checkpoint.position);
    }

    public void ResponePlayer(TestPlayer player, Vector3 checkpointPosition)
    {
        if (player == null)
        {
            return;
        }
        Health health = player.GetComponent<Health>();

        if (health == null || health.IsDead)
        {
            return;
        }
        health.TakeDamage(_returnDamage);
        if (health.IsDead)
        {
            return;
        }
        player.MoveToCheckpoint(checkpointPosition);
    }

    public void RestartPlayer(TestPlayer player, Transform checkpoint)
    {
        if (player == null || checkpoint == null)
        {
            return;
        }
        var point = checkpoint.GetComponent<RespawnPoint>();
        if (point != null)
        {
            player.SetRespawnPoint(point);
        }
        else
        {
            player.SetRespawnPoint(checkpoint.position);
        }
        player.RestartAt(checkpoint.position);
    }

    // Hazard return positions are temporary. Only gates/checkpoints record retry locations.
    public void ResponePlayer(Player player, Transform checkpoint)
    {
        if (checkpoint == null)
        {
            return;
        }
        if (player == null)
        {
            return;
        }
        ResponePlayer(player, checkpoint.position);
        var health = player.GetComponent<Health>();
        if (health != null && !health.IsDead)
        {
            checkpoint.GetComponent<RespawnPoint>()?.RestoreView(player.transform);
        }
    }

    public void ResponePlayer(Player player, Vector3 checkpointPosition)
    {
        if (player == null) return;
        Health health = player.GetComponent<Health>();

        if (health == null || health.IsDead)
        {
            return;
        }
        health.TakeDamage(_returnDamage);
        if (health.IsDead)
        {
            return;
        }
        player.MoveToCheckpoint(checkpointPosition);
    }

    public void RestartPlayer(Player player, Transform checkpoint)
    {
        if (player == null || checkpoint == null) return;
        var point = checkpoint.GetComponent<RespawnPoint>();
        if (point != null)
        {
            player.SetRespawnPoint(point);
        }
        else
        {
            player.SetRespawnPoint(checkpoint.position);
        }
        CheckpointHistory.Record(player, checkpoint);
        player.RestartAt(checkpoint.position);
    }

    // Parameterless entry point for the test player's retry button.
    public void RestartTestPlayer()
    {
        TestPlayer player = GetComponent<TestPlayer>();
        if (player != null)
        {
            player.RestartAtCheckpoint();
        }
    }
}
