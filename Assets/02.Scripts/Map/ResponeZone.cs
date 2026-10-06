using UnityEngine;
using TestPlayer = HyunseoTest.Player;

public class ResponeZone : MonoBehaviour
{
    [SerializeField] private GameObject _checkPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TestPlayer player = other.GetComponentInParent<TestPlayer>();

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
