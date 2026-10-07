using UnityEngine;

public class AirBossDeath : MonoBehaviour
{
    [SerializeField] private Health _airBossossHealth;

    private void OnEnable()
    {
        _airBossossHealth.OnDie += HandleDeath;
    }

    private void OnDisable()
    {
        _airBossossHealth.OnDie -= HandleDeath;
    }

    private void HandleDeath()
    {
    }
}