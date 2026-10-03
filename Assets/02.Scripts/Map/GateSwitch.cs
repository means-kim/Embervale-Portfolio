using System;
using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(BoxCollider2D))]
public class GateSwitch : MonoBehaviour
{
    [Serializable]
    private class Destination
    {
        public CinemachineCamera Camera;
        public bool Inside;
        public bool Outside;
    }

    private enum GateType
    {
        Secret,
        Cave,
        IntoBoss,
        Boss,
        Elite
    }

    [SerializeField] private GateType _gateType;
    [SerializeField] private Destination _left = new Destination();
    [SerializeField] private Destination _right = new Destination();
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private MapVisibility _mapVisibility;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private CinematicManager _cinematicManager;

    [SerializeField] private AreaController _area;
    [SerializeField] private bool _areaIsOnRight = true;
    private bool _enterFromRight;

    private void Awake()
    {
        if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(_playerTag))
        {
            return;
        }

        _enterFromRight = IsRight(other.transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(_playerTag))
        {
            return;
        }

        bool exitFromRight = IsRight(other.transform.position);
        if (_enterFromRight == exitFromRight) return;
        ApplyDestination(exitFromRight ? _right : _left);

        AudioManager audio = AudioManager.Instance;
        if (audio == null) return;

        switch (_gateType)
        {
            case GateType.Cave:
                if (exitFromRight)
                {
                    audio.PlayBgm(BgmType.Cave);
                }
                else
                {
                    audio.StopBgm(BgmType.Cave);
                }

                break;
            case GateType.Elite:
                if (exitFromRight)
                {
                    audio.PlayBgm(BgmType.Ellite);
                }
                else
                {
                    audio.StopBgm(BgmType.Ellite);
                }

                break;

            case GateType.IntoBoss:
                if (exitFromRight)
                {
                    audio.PlayBgm(BgmType.IntoBoss);
                }
                else
                {
                    audio.StopBgm(BgmType.IntoBoss);
                }

                break;

            case GateType.Boss:
                if (exitFromRight)
                {
                    audio.PlayBgm(BgmType.Boss);
                    _cinematicManager.PlayBossEncounter();
                    InArea(_gateType);
                }
                else
                {
                    audio.StopBgm(BgmType.Boss);
                }

                break;
        }
    }

    private void InArea(GateType type)
    {

        switch (type)
        {
            case GateType.Elite:

                break;
            case GateType.Boss:
                break;

        }
    }

    private bool IsRight(Vector3 position)
    {
        Vector3 center = transform.TransformPoint(_boxCollider.offset);
        return position.x >= center.x;
    }

    private void ApplyDestination(Destination destination)
    {
        if (destination.Camera != null)
        {
            CameraManager.SwithCamera(destination.Camera);
        }

        if (_mapVisibility != null)
        {
            _mapVisibility.SetState(
                new MapVisibility.PortalState
                {
                    Inside = destination.Inside,
                    Outside = destination.Outside
                });
        }
    }
}