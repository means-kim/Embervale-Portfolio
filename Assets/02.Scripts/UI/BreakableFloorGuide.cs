using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(200)]
public sealed class BreakableFloorGuide : MonoBehaviour
{
    [SerializeField] private AirComboBreakableCeiling _floor;
    [SerializeField] private PlayerChargeJump _charge;
    [SerializeField] private PlayerAttack _attack;
    [SerializeField] private PlayerJump _jump;
    [SerializeField] private GameObject _sign;
    [SerializeField] private TMP_Text _counter;
    [SerializeField] private Image[] _hitMarks;
    [SerializeField] private CanvasGroup _recovery;
    private int _lastHits = -1;
    private int _lastAirAttacks;
    private bool _wasLaunching;
    private float _hitPulse;
    private float _recoveryTime;

    private void OnEnable()
    {
        _lastHits = -1;
        if (_attack != null)
        {
            _lastAirAttacks = _attack.CurrentAirAttackCount;
        }
        else
        {
            _lastAirAttacks = 0;
        }
        _wasLaunching = _charge != null && _charge.IsLaunching;
        _recoveryTime = 0f;
        if (_recovery != null)
        {
            _recovery.alpha = 0f;
        }
    }

    private void LateUpdate()
    {
        bool visible = _floor != null && _floor.isActiveAndEnabled && !_floor.IsBroken;
        if (_sign != null)
        {
            _sign.SetActive(visible);
        }
        if (!visible)
        {
            if (_recovery != null) _recovery.alpha = 0f;
            return;
        }

        int hits = _floor.HitCount;
        int required = Mathf.Max(1, _floor.RequiredHits);
        if (hits != _lastHits)
        {
            _hitPulse = hits > _lastHits && _lastHits >= 0 ? 0.25f : 0f;
            _counter.text = $"{hits} / {required}";
            _lastHits = hits;
        }
        Color accent = hits == required - 1 ? new Color(1f, 0.85f, 0.35f) : new Color(0.65f, 0.88f, 1f);
        _counter.color = accent;
        _hitPulse = Mathf.Max(0f, _hitPulse - Time.unscaledDeltaTime);
        _counter.transform.localScale = Vector3.one * (1f + _hitPulse * 0.4f);
        for (int i = 0; i < _hitMarks.Length; i++)
        {
            _hitMarks[i].gameObject.SetActive(i < required);
            _hitMarks[i].color = i < hits ? accent : new Color(0.2f, 0.29f, 0.34f, 1f);
        }

        bool launching = _charge != null && _charge.IsLaunching;
        int attacks = _attack != null ? _attack.CurrentAirAttackCount : 0;
        if (launching && !_wasLaunching && attacks < _lastAirAttacks
            && _jump != null && !_jump.IsGrounded
            && Vector2.Distance(_jump.transform.position, _floor.transform.position) < 22f)
        {
            _recoveryTime = 0.7f;
        }
        _lastAirAttacks = attacks;
        _wasLaunching = launching;
        _recoveryTime = Mathf.Max(0f, _recoveryTime - Time.unscaledDeltaTime);
        if (_recovery != null)
        {
            _recovery.alpha = Mathf.Clamp01(_recoveryTime / 0.25f);
            if (_jump != null)
            {
                _recovery.transform.position = _jump.transform.position + Vector3.up * 2.5f;
            }
        }
    }
}
