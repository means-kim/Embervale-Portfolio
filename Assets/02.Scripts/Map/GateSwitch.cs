using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;

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
    [Header("부활 지점")]
    [SerializeField] private RespawnPoint _respawnPoint;
    private bool _enterFromRight;

    private bool _findSecret = false;
    private Coroutine _secretRoutine;
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

        if (exitFromRight)
        {
            var player = other.GetComponentInParent<Player>();
            if (player != null && _respawnPoint != null)
            {
                player.SetRespawnPoint(_respawnPoint);
                CheckpointHistory.Record(player, _respawnPoint.transform);
            }
            InArea(_gateType);
        }

        if (_gateType == GateType.Secret)
        {
            if (exitFromRight)
            {
                CancelSecretOpen();
            }
            else if (!_findSecret && _secretRoutine == null)
            {
                _secretRoutine = StartCoroutine(OpenSecretAfterDelay());
            }
            return;
        }

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
                if (exitFromRight && !EncounterHistory.HasEliminated(EliminateId.Elite01))
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
                    audio.StopBgm(BgmType.Cave);
                    audio.StopBgm(BgmType.Ellite);
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
                    audio.StopBgm(BgmType.Cave);
                    audio.StopBgm(BgmType.Ellite);
                }
                else
                {
                    audio.StopBgm(BgmType.Boss);
                }

                break;
        }
    }

    private IEnumerator OpenSecretAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        UI_Ability ui = UI_Ability.Instance;
        if (ui != null && ui.isActiveAndEnabled && !ui.IsOpen)
        {
            ui.Open();
            _findSecret = ui.IsOpen;
        }
        _secretRoutine = null;
    }

    private void CancelSecretOpen()
    {
        if (_secretRoutine == null) return;
        StopCoroutine(_secretRoutine);
        _secretRoutine = null;
    }

    private void OnDisable()
    {
        CancelSecretOpen();
    }

    private void InArea(GateType type)
    {
        switch (type)
        {
            case GateType.Elite:
                if (_area == null)
                {
                    return;
                }

                _area.TryEnter();
                break;
            case GateType.Boss:
                if (_area == null)
                {
                    return;
                }

                _area.TryEnter();
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
