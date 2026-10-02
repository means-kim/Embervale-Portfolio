using System.Collections.Generic;
using Team08.UI;
using UnityEngine;

public class UI_Health : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private HealthFlameGraphic _flamePrefab;
    [SerializeField] private Transform _slot;

    private readonly List<HealthFlameGraphic> _flames = new();

    private void Start()
    {
        if (_health == null || _flamePrefab == null || _slot == null)
        {
            enabled = false;
            return;
        }

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_health == null)
        {
            return;
        }

        int count = Mathf.CeilToInt(Mathf.Max(0, _health.MaxHealth) / 100f);
        while (_flames.Count < count)
        {
            HealthFlameGraphic flame = Instantiate(_flamePrefab, _slot);
            flame.Phase = _flames.Count * 1.7f;
            _flames.Add(flame);
        }

        while (_flames.Count > count)
        {
            int last = _flames.Count - 1;
            HealthFlameGraphic flame = _flames[last];
            _flames.RemoveAt(last);
            flame.gameObject.SetActive(false);
            Destroy(flame.gameObject);
        }

        float health = Mathf.Clamp(_health.CurrentHealth, 0, Mathf.Max(0, _health.MaxHealth)) / 100f;
        for (int i = 0; i < _flames.Count; i++)
        {
            float fill = Mathf.Clamp01(health - i);
            if (_flames[i].Fill != fill)
            {
                _flames[i].Fill = fill;
            }
        }
    }
}