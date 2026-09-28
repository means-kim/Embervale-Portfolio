using System;
using System.Collections.Generic;
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

    [SerializeField] private Destination _left = new Destination();
    [SerializeField] private Destination _right = new Destination();
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private MapVisibility _mapVisibility;
    [SerializeField] private BoxCollider2D _boxCollider;

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

        _enterFromRight = IsRight(other.GetComponent<Transform>().position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(_playerTag))
        {
            return;
        }

        bool exitFromRight = IsRight(other.GetComponent<Transform>().position);
        if (_enterFromRight == exitFromRight) return;
        ApplyDestination(exitFromRight ? _right : _left);
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