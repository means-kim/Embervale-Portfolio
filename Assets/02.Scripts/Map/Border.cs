using System;
using UnityEngine;

public class Border : MonoBehaviour
{
    public event Action PlayerDeath;
    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTag("Player"))
        {
            PlayerDeath?.Invoke();
        }
    }
}
