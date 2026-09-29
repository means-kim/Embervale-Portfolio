using System.Collections.Generic;
using UnityEngine;
public class BuffPool : MonoBehaviour
{
    [SerializeField] private List<BuffSO> _pool;
    private readonly List<BuffSO> _activeBuffs = new();
    public IReadOnlyList<BuffSO> ActiveBuffs => _activeBuffs;
    public void ClearBuffs() => _activeBuffs.Clear();
    public BuffSO AddRandomBuff()
    {
        if (_pool == null || _pool.Count == 0) return null;
        BuffSO picked = null;
        List<BuffSO> eligible = new(_pool.Count);
        foreach (var candidate in _pool)
        {
            if (candidate == null) continue;
            if (IsCapped(candidate)) continue;
            eligible.Add(candidate);
        }
        if (eligible.Count == 0) return null;
        int totalWeight = 0;
        foreach (var candidate in eligible)
        {
            totalWeight += Mathf.Max(0, candidate.Weight);
        }
        if (totalWeight <= 0)
        {
            picked = eligible[Random.Range(0, eligible.Count)];
        }
        else
        {
            int roll = Random.Range(0, totalWeight);
            int accumulated = 0;
            foreach (var candidate in eligible)
            {
                accumulated += Mathf.Max(0, candidate.Weight);
                if (roll < accumulated)
                {
                    picked = candidate;
                    break;
                }
            }
            if (picked == null) picked = eligible[eligible.Count - 1];
        }

        _activeBuffs.Add(picked);
        return picked;
    }
    public bool IsCapped(BuffSO buff)
    {
        if (buff == null) return true;
        if (buff.MaxNumberOfBuffs <= 0) return false;
        return GetActiveCount(buff) >= buff.MaxNumberOfBuffs;
    }
    public int GetActiveCount(BuffSO buff)
    {
        if (buff == null) return 0;
        int count = 0;
        foreach (var d in _activeBuffs)
        {
            if (ReferenceEquals(d, buff)) count++;
        }
        return count;
    }
    public float GetTotalValue(Buff buffType)
    {
        float total = 0;
        foreach (var d in _activeBuffs)
        {
            if (d == null) continue;
            if (d.BuffType == buffType)
            {
                total += d.Value;
            }
        }
        return total;
    }

#if UNITY_EDITOR
    // 버프당 SO 1개 규칙 검사. 같은 BuffType이 풀에 2개 이상이면 캡이 뚫리므로 경고한다.
    private void OnValidate()
    {
        if (_pool == null) return;
        var seen = new HashSet<Buff>();
        var duplicated = new HashSet<Buff>();
        foreach (var candidate in _pool)
        {
            if (candidate == null) continue;
            if (!seen.Add(candidate.BuffType)) duplicated.Add(candidate.BuffType);
        }
        foreach (var buffType in duplicated)
        {
            int count = 0;
            foreach (var candidate in _pool)
            {
                if (candidate != null && candidate.BuffType == buffType) count++;
            }
            Debug.LogWarning($"[BuffPool] {name}: {buffType} 타입 SO가 {count}개 등록됨. 버프당 SO 1개 규칙 위반.", this);
        }
    }
#endif
}