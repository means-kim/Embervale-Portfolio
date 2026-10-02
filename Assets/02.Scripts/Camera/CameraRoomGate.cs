using Unity.Cinemachine;
using UnityEngine;
public class CameraRoomGate : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _leftCamera;
    [SerializeField] private CinemachineCamera _rightCamera;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActiveAndEnabled) return;
        if (!collision.CompareTag("Player")) return;
        float playerX = collision.transform.position.x;
        float gateX = transform.position.x;
        if (playerX < gateX)
        {
            CameraManager.SwithCamera(_rightCamera);
        }
        else
        {
            CameraManager.SwithCamera(_leftCamera);
        }
    }
}
