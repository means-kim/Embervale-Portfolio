using UnityEngine;

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
        _jump.OnJumpStarted += HandleAirStart;
        _jump.OnGroundLost += HandleAirStartFromFall;
        _jump.OnLanded += HandleLanded;
    }

    private void OnDisable()
    {
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
        if (_airStartTime <= -99f)
        {
            _airStartTime = Time.time;
        }
    }

    private void HandleLanded()
    {
        float airTime = Time.time - _airStartTime;
        _airStartTime = -100f;
        if (airTime < _minAirTime)
        {
            return;
        }
        EffectManager.Instance?.PlayLand(transform.position + _positionOffset);
    }
}
