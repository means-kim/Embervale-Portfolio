using UnityEngine;

/// <summary>발걸음마다 생성하는 한 번 재생용 파티클. 속도는 플레이어 뒤쪽으로 설정한다.</summary>
[RequireComponent(typeof(ParticleSystem), typeof(OneShotVFX))]
public class FootstepParticle : MonoBehaviour
{
    [SerializeField, Tooltip("뒤쪽으로 흩날리는 속도의 최소/최대값 (월드 단위/초).")]
    private Vector2 _backwardSpeed = new(0.3f, 0.7f);
    [SerializeField, Tooltip("위로 흩날리는 속도의 최소/최대값 (월드 단위/초).")]
    private Vector2 _upwardSpeed = new(0.1f, 0.3f);

    public void Play(int facingDirection)
    {
        var particles = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var particle in particles)
        {
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            var velocity = particle.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            float min = Mathf.Max(0f, Mathf.Min(_backwardSpeed.x, _backwardSpeed.y));
            float max = Mathf.Max(min, Mathf.Max(_backwardSpeed.x, _backwardSpeed.y));
            velocity.x = facingDirection < 0
                ? new ParticleSystem.MinMaxCurve(min, max)
                : new ParticleSystem.MinMaxCurve(-max, -min);
            velocity.y = new ParticleSystem.MinMaxCurve(
                Mathf.Min(_upwardSpeed.x, _upwardSpeed.y), Mathf.Max(_upwardSpeed.x, _upwardSpeed.y));
            velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        }
        foreach (var particle in particles)
            if (particle.gameObject.activeInHierarchy) particle.Play(false);
    }
}
