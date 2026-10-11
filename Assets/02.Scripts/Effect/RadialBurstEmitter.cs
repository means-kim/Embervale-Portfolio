using UnityEngine;
using Game.Effects;

/// <summary>
/// ParticleSystem을 "한 번에 터뜨리는" 용도로만 쓰는 래퍼. (원형 / 가로 방향 지원)
/// Shape 모듈 대신 EmitParams로 직접 속도를 넣는 이유:
/// 원본처럼 가로로 납작한 분포(verticalSquash)와 개별 속도 범위를 Settings 하나로 정확히 제어하기 위해서다.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public sealed class RadialBurstEmitter : MonoBehaviour
{
    private const int SheetTilesX = 2;
    private const int SheetTilesY = 2;

    private ParticleSystem _particleSystem;

    private ParticleSystem System
    {
        get
        {
            // 에디터 빌더에서 Awake 전에 호출될 수 있으므로 지연 캐싱.
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            return _particleSystem;
        }
    }

    /// <summary>
    /// Settings 값으로 ParticleSystem 모듈을 구성한다.
    /// 튜닝은 Inspector의 파티클 모듈이 아니라 Settings 에셋에서 한다(런타임에 덮어씀).
    /// </summary>
    public void Configure(BurstSettings burst, string sortingLayerName, int sortingOrder)
    {
        ParticleSystem ps = System;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = ps.main;
        main.loop = false;
        main.useUnscaledTime = true;
        main.playOnAwake = false;
        main.duration = 1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        main.gravityModifier = 0f;
        main.startSpeed = 0f;
        main.startColor = burst.color;
        // 연속 피격 대비 2회분 여유.
        main.maxParticles = Mathf.Max(1, burst.count * 2);

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = false;

        ParticleSystem.LimitVelocityOverLifetimeModule limit = ps.limitVelocityOverLifetime;
        limit.enabled = burst.drag > 0f;
        limit.limit = 100000f; // 사실상 속도 제한 없이 drag만 사용
        limit.drag = burst.drag;
        limit.multiplyDragByParticleSize = false;
        limit.multiplyDragByParticleVelocity = false;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = CreateFadeGradient(burst.fadeStart);

        ParticleSystem.RotationOverLifetimeModule rotation = ps.rotationOverLifetime;
        rotation.enabled = burst.angularSpeed > 0f;
        float angular = burst.angularSpeed * Mathf.Deg2Rad;
        rotation.z = new ParticleSystem.MinMaxCurve(-angular, angular);

        // 2x2 시트에서 파티클마다 랜덤 프레임 하나를 고정으로 사용.
        ParticleSystem.TextureSheetAnimationModule sheet = ps.textureSheetAnimation;
        sheet.enabled = true;
        sheet.mode = ParticleSystemAnimationMode.Grid;
        sheet.numTilesX = SheetTilesX;
        sheet.numTilesY = SheetTilesY;
        sheet.animation = ParticleSystemAnimationType.WholeSheet;
        sheet.frameOverTime = new ParticleSystem.MinMaxCurve(0f, 0.999f);
        sheet.cycleCount = 1;

        var particleRenderer = GetComponent<ParticleSystemRenderer>();
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.sortingLayerName = sortingLayerName;
        particleRenderer.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// center에서 방사형으로 burst.count개의 파티클을 즉시 방출한다. (GC 할당 없음)
    /// </summary>
    public void Emit(Vector3 center, BurstSettings burst, float scale)
    {
        if (burst.count <= 0)
        {
            return;
        }

        ParticleSystem ps = System;
        if (!ps.isPlaying)
        {
            ps.Play(false); // emission이 꺼져 있으므로 자동 방출은 없다.
        }

        var emitParams = new ParticleSystem.EmitParams();
        for (int i = 0; i < burst.count; i++)
        {
            Vector3 direction = SampleDirection(burst);
            Vector2 jitter = Random.insideUnitCircle * (burst.spawnJitter * scale);

            emitParams.position = center + new Vector3(jitter.x, jitter.y, 0f);
            emitParams.velocity = direction * (RandomIn(burst.speedRange) * scale);
            emitParams.startSize = RandomIn(burst.sizeRange) * scale;
            emitParams.startLifetime = RandomIn(burst.lifetimeRange);
            emitParams.rotation = Random.Range(0f, 360f);
            ps.Emit(emitParams, 1);
        }
    }

    private static float RandomIn(Vector2 range) => Random.Range(range.x, range.y);

    /// <summary>방향 모드에 따라 단위 방향(세로 squash 적용)을 뽑는다.</summary>
    private static Vector3 SampleDirection(BurstSettings burst)
    {
        float angle;
        if (burst.direction == BurstDirection.Horizontal)
        {
            float side = Random.value < 0.5f ? 0f : Mathf.PI; // 오른쪽 / 왼쪽
            angle = side + Random.Range(-burst.spreadAngle, burst.spreadAngle) * Mathf.Deg2Rad;
        }
        else
        {
            angle = Random.Range(0f, Mathf.PI * 2f);
        }

        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) * burst.verticalSquash, 0f);
    }

    private static Gradient CreateFadeGradient(float fadeStart)
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, Mathf.Clamp(fadeStart, 0.01f, 0.99f)),
                new GradientAlphaKey(0f, 1f),
            });
        return gradient;
    }
}
