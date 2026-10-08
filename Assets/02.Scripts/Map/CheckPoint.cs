using UnityEngine;
public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        Player player = other.GetComponent<Player>();
        if (player == null) return;
        player.SetRespawnPoint(transform.position);
        CheckpointHistory.Record(player, transform);
        EffectManager.Instance?.PlayCheckpoint(transform.position);
    }
}
