using System;
using UnityEngine;
namespace HyunseoTest
{
    public class PlayerLevel : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _level = 1;
        [SerializeField, Min(1)] private int _maxLevel = 99;
        public int Level => _level;
        public int MaxLevel => _maxLevel;
        public event Action<int> OnLevelChanged;
        public void LevelUp() => AddLevel(1);
        public void AddLevel(int amount)
        {
            if (amount <= 0) return;
            int next = Mathf.Min(_level + amount, _maxLevel);
            int gained = next - _level;
            if (gained <= 0) return;
            _level = next;
            // Global buffs are deliberately not modified by the isolated test player.
            OnLevelChanged?.Invoke(_level);
        }
    }

}
