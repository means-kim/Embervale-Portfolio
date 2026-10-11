using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 플레이어가 적을 때렸을 때의 타격 연출 오케스트레이터.
/// 타임라인:
///   [0f]   짧은 히트스톱, 적을 관통하는 흰 섬광선, 타격점 아래 가시선
///   [~4f]  섬광선이 얇아지며 사라짐, 가시선 페이드
/// </summary>
/// <remarks>
/// 인스턴스 하나를 재사용하며 연속 타격 시 처음부터 다시 시작한다.
/// 여러 적을 같은 프레임에 동시에 때리는 구조라면 인스턴스를 적 수만큼 두거나 풀링해서 쓴다.
/// </remarks>
public sealed class EnemyHitEffect : MonoBehaviour
{
    [FormerlySerializedAs("settings")]
    [SerializeField] private AttackEffectSettings _settings;

    [Header("Visual Parts (빌더가 자동 연결)")]
    [FormerlySerializedAs("streakRenderer")]
    [SerializeField] private SpriteRenderer _streakRenderer;
    [FormerlySerializedAs("spikeRenderers")]
    [SerializeField] private SpriteRenderer[] _spikeRenderers;

    public bool IsPlaying => _routine != null;

    private Coroutine _routine;
    private Vector3 _streakBaseScale;

    private void Awake()
    {
        ApplySettings();
        HideAll();
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        HideAll();
    }

    /// <summary>
    /// 타격 연출을 재생한다. 재생 중이면 처음부터 다시 시작한다.
    /// </summary>
    /// <param name="hitPoint">타격 지점(보통 적 콜라이더의 가장 가까운 점이나 중심).</param>
    /// <param name="hitDirection">공격 방향(플레이어 → 적). 섬광선/가시선 배치의 기준.</param>
    public void Play(Vector3 hitPoint, Vector2 hitDirection)
    {
        if (_settings == null)
        {
            return;
        }

        if (hitDirection.sqrMagnitude < 0.0001f)
        {
            hitDirection = Vector2.right;
        }

        hitDirection.Normalize();

        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _routine = StartCoroutine(PlayRoutine(hitPoint, hitDirection));
    }

    /// <summary>Settings 값을 크기/정렬에 반영한다.</summary>
    public void ApplySettings()
    {
        if (_settings == null)
        {
            return;
        }

        AttackEffectSettings s = _settings;
        float scale = s.globalScale;

        _streakBaseScale = ScaleForWorldSize(
            _streakRenderer, new Vector2(s.streakLength, s.streakWidth) * scale);

        SetSorting(_streakRenderer, s.sortingLayerName, s.streakSortingOrder);

        for (int i = 0; i < _spikeRenderers.Length; i++)
        {
            SetSorting(_spikeRenderers[i], s.sortingLayerName, s.streakSortingOrder);
        }
    }

    private IEnumerator PlayRoutine(Vector3 hitPoint, Vector2 direction)
    {
        AttackEffectSettings s = _settings;
        float scale = s.globalScale;
        transform.position = hitPoint;

        // ---- 즉시 발생 ----------------------------------------------------
        HitImpactFeedback.EnsureInstance().RequestHitstop(s.hitStopDuration, s.hitStopTimeScale);
        CameraManager.ShakeCamera(s.shakeAmplitude * scale, s.shakeDuration);

        PlaceStreak(s, direction);
        int spikeCount = PlaceSpikes(s, direction, scale);

        _streakRenderer.enabled = true;

        // ---- 시간 진행 (unscaled: 히트스톱 중에도 섬광선이 보여야 함) ----
        float duration = s.ImpactDuration;
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            UpdateStreak(s, t);
            UpdateSpikes(s, t, spikeCount);
            yield return null;
        }

        HideAll();
        _routine = null;
    }

    private void PlaceStreak(AttackEffectSettings s, Vector2 direction)
    {
        // 타격 방향 기준으로 "위쪽"으로 살짝 기울인다. 왼쪽 공격이면 회전 부호가 반대.
        float upSign = direction.x >= 0f ? 1f : -1f;
        float tilt = Random.Range(s.streakTiltRange.x, s.streakTiltRange.y) * upSign;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + tilt;
        _streakRenderer.transform.localPosition = Vector3.zero;
        _streakRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    /// <returns>실제로 배치한 가시 개수.</returns>
    private int PlaceSpikes(AttackEffectSettings s, Vector2 direction, float scale)
    {
        int count = Mathf.Min(s.spikeCount, _spikeRenderers.Length);
        Vector2 along = direction;
        Vector2 down = Vector2.down;

        for (int i = 0; i < count; i++)
        {
            SpriteRenderer spike = _spikeRenderers[i];
            var area = new Vector2(
                Random.Range(s.spikeAreaMin.x, s.spikeAreaMax.x),
                Random.Range(s.spikeAreaMin.y, s.spikeAreaMax.y));
            Vector2 offset = (along * area.x + down * area.y) * scale;
            spike.transform.localPosition = offset;

            // 대부분 "/" 방향, 가끔 "\" 방향으로 섞어 단조로움을 피한다.
            float sign = Random.value < 0.75f ? 1f : -1f;
            float spikeAngle = Random.Range(s.spikeAngleRange.x, s.spikeAngleRange.y) * sign;
            spike.transform.localRotation = Quaternion.Euler(0f, 0f, spikeAngle);

            float length = s.spikeLength * Random.Range(s.spikeLengthJitter.x, s.spikeLengthJitter.y);
            spike.transform.localScale = ScaleForWorldSize(
                spike, new Vector2(length, s.spikeWidth) * scale);
            spike.enabled = true;
        }

        for (int i = count; i < _spikeRenderers.Length; i++)
        {
            _spikeRenderers[i].enabled = false;
        }

        return count;
    }

    private void UpdateStreak(AttackEffectSettings s, float t)
    {
        float grow = Mathf.Lerp(0.7f, 1f, Mathf.Clamp01(t / s.streakGrowDuration));
        float thickness = 1f;
        float alpha = 1f;

        if (t > s.streakHoldDuration)
        {
            float k = Mathf.Clamp01((t - s.streakHoldDuration) / s.streakFadeDuration);
            thickness = 1f - s.streakThinOnFade * k;
            alpha = 1f - k;
        }

        Vector3 streakScale = _streakBaseScale;
        streakScale.x *= grow;
        streakScale.y *= thickness;
        _streakRenderer.transform.localScale = streakScale;
        SetTint(_streakRenderer, s.streakColor, alpha);
    }

    private void UpdateSpikes(AttackEffectSettings s, float t, int count)
    {
        float alpha = 1f - Mathf.Clamp01(t / s.spikeFadeDuration);
        for (int i = 0; i < count; i++)
        {
            SetTint(_spikeRenderers[i], s.spikeColor, alpha);
        }
    }

    private static Vector3 ScaleForWorldSize(SpriteRenderer renderer, Vector2 size)
    {
        if (renderer == null || renderer.sprite == null)
        {
            return Vector3.one;
        }
        Vector2 spriteSize = renderer.sprite.bounds.size;
        return new Vector3(size.x / spriteSize.x, size.y / spriteSize.y, 1f);
    }

    private static void SetSorting(Renderer renderer, string layer, int order)
    {
        if (renderer == null)
        {
            return;
        }
        renderer.sortingLayerName = layer;
        renderer.sortingOrder = order;
    }

    private static void SetTint(SpriteRenderer renderer, Color color, float alpha)
    {
        color.a *= alpha;
        renderer.color = color;
    }
    private void HideAll()
    {
        if (_streakRenderer != null)
        {
            _streakRenderer.enabled = false;
        }
        if (_spikeRenderers == null)
        {
            return;
        }

        foreach (SpriteRenderer spike in _spikeRenderers)
        {
            if (spike != null) spike.enabled = false;
        }
    }
}
