using UnityEngine;

/// <summary>
/// 착지 시 EffectManager.PlayLand를 호출한다. 점프/낙하 시작~착지 사이
/// 체공 시간이 _minAirTime보다 짧으면(턱 오르내리기 등 미세 착지) 재생하지 않는다.
/// 발 위치 보정은 보통 VFX 프리팹의 VFXOffset에서 하고, 여기는 0 유지를 권장한다.
/// </summary>
[RequireComponent(typeof(PlayerJump))]
public class PlayerLandEffect : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField, Min(0f), Tooltip("이 시간보다 짧은 체공 후 착지는 이펙트 없음. 미세 착지 깜빡임 방지.")]
    private float _minAirTime = 0.12f;
    [Header("Spawn")]
    [SerializeField, Tooltip("생성 위치 보정. 대부분은 프리팹의 VFXOffset에서 조정하고 여기는 0 유지.")]
    private Vector3 _positionOffset;
    private PlayerJump _jump;
    private float _airStartTime = -100f;

    private void Awake()
    {
        _jump = GetComponent<PlayerJump>();
    }

    private void OnEnable()
    {
        if (_jump == null) return;
        _jump.OnJumpStarted += HandleAirStart;
        _jump.OnGroundLost += HandleAirStartFromFall;
        _jump.OnLanded += HandleLanded;
    }

    private void OnDisable()
    {
        if (_jump == null) return;
        _jump.OnJumpStarted -= HandleAirStart;
        _jump.OnGroundLost -= HandleAirStartFromFall;
        _jump.OnLanded -= HandleLanded;
    }

    public void ResetForRespawn()
    {
        _airStartTime = -100f;
    }

    private void HandleAirStart()
    {
        _airStartTime = Time.time;
    }

    private void HandleAirStartFromFall()
    {
        // 점프 입력 없이 낭떠러지에서 걸어 떨어진 경우만 기록한다.
        // 점프로 뜬 뒤에는 HandleAirStart가 이미 더 이른 시각을 잡았다.
        if (_airStartTime <= -99f) _airStartTime = Time.time;
    }

    private void HandleLanded()
    {
        float airTime = Time.time - _airStartTime;
        _airStartTime = -100f;
        if (airTime < _minAirTime) return;
        EffectManager.Instance?.PlayLand(transform.position + _positionOffset);
    }
}
