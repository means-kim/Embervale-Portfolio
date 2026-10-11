using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[DefaultExecutionOrder(100)]
public sealed class BossStaggerBar : MonoBehaviour
{
    [SerializeField] private AirBossStagger _stagger;
    [SerializeField] private Image _fill;

    private void OnEnable() => Refresh();

    private void LateUpdate() => Refresh();

    private void Refresh()
    {
        if (_fill == null) return;
        _fill.fillAmount = _stagger != null && _stagger.isActiveAndEnabled
            ? Mathf.Clamp01(_stagger.NormalizedGauge)
            : 0f;
    }
}
