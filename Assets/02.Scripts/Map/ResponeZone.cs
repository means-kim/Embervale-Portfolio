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

        Respone.Instance.ResponePlayer(player, _checkPoint.transform);
    }
}
