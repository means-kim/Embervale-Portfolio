using Unity.Cinemachine;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private MapVisibility _mapVisibility;
    [SerializeField] private bool _inside;
    [SerializeField] private bool _outside;
    [SerializeField] private AreaController _area;
    public AreaController BattleArea => _area;

    public void PrepareForRetry() => _area?.ResetForRetry();

    public void RestoreView(Transform player)
    {
        if (player == null) return;
        if (_mapVisibility != null)
        {
            _mapVisibility.SetState(new MapVisibility.PortalState
            {
                Inside = _inside,
                Outside = _outside
            });
        }

        if (_camera == null)
        {
            return;
        }

        _camera.Follow = player;
        _camera.PreviousStateIsValid = false;
        CameraManager.SwithCamera(_camera);
    }
}
