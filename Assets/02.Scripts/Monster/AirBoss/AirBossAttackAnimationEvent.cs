using System;
using UnityEngine;

public class AirBossAttackAnimationEvent : MonoBehaviour
{
    private AirBossAttack _attack;
    private Collider2D _collider;

    private void Awake()
    {
        _attack = GetComponentInParent<AirBossAttack>();

        if (_attack != null)
        {
            _collider = _attack.GetComponent<Collider2D>();
        }
    }

    private void Release()
    {
        if (_attack != null)
        {
            _attack.Release();
        }
    }

    private void EnableHitBox()
    {
        if (_collider != null)
        {
            _collider.enabled = true;
            // Debug.Log("콜라이더 켜짐");
        }
    }

    private void DisableHitBox()
    {
        if (_collider != null)
        {
            _collider.enabled = false;
            // Debug.Log("콜라이더 꺼짐");
        }
    }

    private void OnEnable()
    {
        DisableHitBox();
    }

    private void OnDisable()
    {
        DisableHitBox();
    }
}