using System;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    public event Action PlayerDeath;
    [SerializeField, Min(0)] private int _returnDamage = 50;
    [SerializeField, Min(0.05f)] private float _clearContactTime = 0.25f;

    private sealed class ContactState
    {
        public int count;
        public bool damaged;
        public float lastExit = float.NegativeInfinity;
        public float clearTime;
    }

    // Shared per player so adjacent spike pieces cannot each apply damage on the same touch.
    private static readonly Dictionary<Health, ContactState> PlayerContacts = new();
    private readonly Dictionary<Collider2D, Health> _contacts = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetContactStates() => PlayerContacts.Clear();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RegisterContact(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision) => RegisterContact(collision.collider);
    private void OnCollisionExit2D(Collision2D collision) => RemoveContact(collision.collider);

    private void RegisterContact(Collider2D other)
    {
        if (!isActiveAndEnabled || other == null || _contacts.ContainsKey(other)) return;
        var body = other.attachedRigidbody;
        if (!other.CompareTag("Player") && (body == null || !body.CompareTag("Player"))) return;
        Health health = other.GetComponentInParent<Health>();
        if (health == null || health.IsDead) return;
        if (!PlayerContacts.TryGetValue(health, out var state))
        {
            state = new ContactState();
            PlayerContacts.Add(health, state);
        }

        if (state.count == 0 && Time.time - state.lastExit >= state.clearTime)
        {
            state.damaged = false;
        }

        state.count++;
        state.clearTime = Mathf.Max(state.clearTime, _clearContactTime);
        _contacts.Add(other, health);
        if (state.damaged)
        {
            return;
        }

        state.damaged = true;
        PlayerDeath?.Invoke();
        if (health != null && !health.IsDead)
        {
            health.TakeDamage(_returnDamage);
        }
    }

    private void RemoveContact(Collider2D other)
    {
        if (!_contacts.TryGetValue(other, out var health))
        {
            return;
        }

        _contacts.Remove(other);
        if (PlayerContacts.TryGetValue(health, out var state))
        {
            state.count = Mathf.Max(0, state.count - 1);
            if (state.count == 0)
            {
                state.lastExit = Time.time;
            }
        }
    }

    private void OnDisable()
    {
        foreach (var other in new List<Collider2D>(_contacts.Keys))
        {
            RemoveContact(other);
        }
    }
}