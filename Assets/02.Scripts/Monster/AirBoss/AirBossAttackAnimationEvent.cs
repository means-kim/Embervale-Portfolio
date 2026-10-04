using System;
using UnityEngine;

public class AirBossAttackAnimationEvent : MonoBehaviour
{
    private AirBossAttack _attack;

    private void Awake()
    {
        _attack = GetComponent<AirBossAttack>();
    }

    private void Release()
    {
        if (_attack != null)
        {
            _attack.Release();
        }
    }
}