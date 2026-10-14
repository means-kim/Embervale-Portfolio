using System;
using System.Collections;
using System.Collections.Generic;
using Game.Effects;
using UnityEngine;

// 피격 효과의 실제 시간 재생과 수명 관리. 코루틴은 기존 매니저가 실행한다.
internal sealed class HitEffectService
{
    private readonly List<GameObject> _effects = new();

    public void PlayPlayerHit(EffectManager host, GameObject prefab, Vector3 position, Action onReleased)
    {
        if (!host.isActiveAndEnabled)
        {
            onReleased?.Invoke();
            return;
        }

        GameObject effect = Spawn(prefab, position);
        if (effect != null && effect.TryGetComponent<PlayerHitEffect>(out var hitEffect))
        {
            Track(host, effect, hitEffect);
            hitEffect.Play(position, onReleased);
        }
        else
        {
            if (prefab == null)
            {
                Debug.LogWarning("EffectManager에 플레이어 피격 프리팹이 연결되지 않았습니다.", host);
            }

            HitImpactFeedback.EnsureInstance().RequestPlayerHitstop();
            if (effect != null)
            {
                Track(host, effect, null);
            }

            onReleased?.Invoke();
        }
    }

    public void PlayBossHit(EffectManager host, GameObject prefab, Vector3 position, Vector2 direction)
    {
        if (!host.isActiveAndEnabled)
        {
            return;
        }

        GameObject effect = Spawn(prefab, position);
        if (effect != null && effect.TryGetComponent<EnemyHitEffect>(out var enemyHit))
        {
            enemyHit.Play(position, direction);
        }
        else
        {
            HitImpactFeedback.EnsureInstance().RequestBossHitstop();
        }

        if (effect != null)
        {
            Track(host, effect, null);
        }
    }

    public void PlayBossHit(
        EffectManager host,
        GameObject prefab,
        Vector3 position,
        Vector2 direction,
        int damage,
        Transform target,
        bool isThirdComboHit,
        BossDamageText damageTextPrefab,
        GameObject thirdHitPrefab)
    {
        if (!host.isActiveAndEnabled)
        {
            return;
        }

        if (damage > 0 && damageTextPrefab != null)
        {
            Vector3 textPosition = position + Vector3.up * 5f;

            BossDamageText damageText = UnityEngine.Object.Instantiate(
                damageTextPrefab,
                textPosition,
                Quaternion.identity);

            _effects.Add(damageText.gameObject);

            damageText.Play(damage, textPosition, isThirdComboHit);
            host.StartCoroutine(CleanupDamageText(damageText));
        }

        // 기존 PlayerHitEffect 컴포넌트를 사용하는 보스 전용 프리팹
        if (isThirdComboHit && thirdHitPrefab != null)
        {
            PlayPlayerHit(host, thirdHitPrefab, position, null);
        }
        else
        {
            // 일반 타격 또는 3타 프리팹 미연결: 기본 효과 출력
            PlayBossHit(host, prefab, position, direction);
        }
    }

    private IEnumerator CleanupDamageText(BossDamageText damageText)
    {
        yield return null;

        while (damageText != null && damageText.IsPlaying)
        {
            yield return null;
        }

        if (damageText != null)
        {
            GameObject effect = damageText.gameObject;
            _effects.Remove(effect);
            UnityEngine.Object.Destroy(effect);
        }
    }

    public void PlayPlayerFloatingText(
        EffectManager host,
        PlayerFloatingText prefab,
        Vector3 position,
        int amount,
        bool plusPrefix)
    {
        if (!host.isActiveAndEnabled || prefab == null || amount <= 0)
        {
            return;
        }

        PlayerFloatingText floatingText = UnityEngine.Object.Instantiate(
            prefab,
            position,
            Quaternion.identity);

        _effects.Add(floatingText.gameObject);

        floatingText.Play(amount, position, plusPrefix);
        host.StartCoroutine(CleanupPlayerText(floatingText));
    }

    private IEnumerator CleanupPlayerText(PlayerFloatingText floatingText)
    {
        yield return null;

        while (floatingText != null && floatingText.IsPlaying)
        {
            yield return null;
        }

        if (floatingText != null)
        {
            GameObject effect = floatingText.gameObject;
            _effects.Remove(effect);
            UnityEngine.Object.Destroy(effect);
        }
    }

    public void Clear()
    {
        foreach (GameObject effect in _effects)
        {
            if (effect != null) UnityEngine.Object.Destroy(effect);
        }

        _effects.Clear();
    }

    private static GameObject Spawn(GameObject prefab, Vector3 position) =>
        prefab != null ? UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity) : null;

    private void Track(EffectManager host, GameObject effect, PlayerHitEffect hitEffect)
    {
        _effects.Add(effect);
        foreach (ParticleSystem particle in effect.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = particle.main;
            main.useUnscaledTime = true;
        }

        foreach (Animator animator in effect.GetComponentsInChildren<Animator>(true))
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        host.StartCoroutine(Cleanup(effect, hitEffect));
    }

    private IEnumerator Cleanup(GameObject effect, PlayerHitEffect hitEffect)
    {
        yield return null;
        EnemyHitEffect enemyHit = effect != null ? effect.GetComponent<EnemyHitEffect>() : null;
        if (hitEffect == null && enemyHit == null)
        {
            yield return new WaitForSecondsRealtime(2f);
        }

        ParticleSystem[] particles = effect != null
            ? effect.GetComponentsInChildren<ParticleSystem>(true)
            : Array.Empty<ParticleSystem>();
        while (effect != null)
        {
            bool alive = false;
            if (enemyHit != null)
            {
                alive = enemyHit.IsPlaying;
            }

            if (hitEffect != null)
            {
                alive = hitEffect.IsPlaying;
            }

            foreach (ParticleSystem particle in particles)
            {
                if (particle != null && particle.IsAlive(true))
                {
                    alive = true;
                }
            }

            if (!alive)
            {
                break;
            }

            yield return null;
        }

        _effects.Remove(effect);
        if (effect != null)
        {
            UnityEngine.Object.Destroy(effect);
        }
    }
}
