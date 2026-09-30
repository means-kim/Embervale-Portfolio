using System;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField, Min(1)] private int _level = 1;
    [SerializeField, Min(1)] private int _maxLevel = 99;
    [SerializeField] private BuffPool _buffPool;

    public int Level => _level;
    public int MaxLevel => _maxLevel;

    // 레벨이 올랐을 때의 행동은 나중에 이 이벤트에 연결한다.
    public event Action<int> OnLevelChanged;

    public void LevelUp() => AddLevel(1);

    public void AddLevel(int amount)
    {
        if (amount <= 0) return;
        int next = Mathf.Min(_level + amount, _maxLevel);
        int gained = next - _level;
        if (gained <= 0) return;
        _level = next;
        // 오른 레벨 1당 랜덤 버프 1개를 적용한다.
        ApplyLevelUpBuffs(gained);
        OnLevelChanged?.Invoke(_level);
    }

    private void ApplyLevelUpBuffs(int gained)
    {
        if (_buffPool == null)
            _buffPool = FindFirstObjectByType<BuffPool>();
        if (_buffPool == null) return;
        for (int i = 0; i < gained; i++)
            _buffPool.AddRandomBuff();
    }
}
