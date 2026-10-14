using UnityEngine;

public class ResponeZone : MonoBehaviour
{
    [SerializeField] private GameObject _checkPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player == null || _checkPoint == null)
        {
            return;
        }

        if (Respone.Instance == null)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsRecentRetry)
        {
            Debug.LogWarning("ResponeZone: 재시작 직후 위험 구역 진입. zone=" + name +
                             ", position=" + player.transform.position + ", destination=" +
                             _checkPoint.transform.position + ", dead=" + player.GetComponent<Health>().IsDead, this);
        }

        Respone.Instance.ResponePlayer(player, _checkPoint.transform);
    }
}
