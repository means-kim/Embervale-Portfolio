using UnityEngine;

namespace Game.UI.Boss
{
    [RequireComponent(typeof(BossHealthBar))]
    public sealed class BossHealthBinding : MonoBehaviour
    {
        [SerializeField] private Health _bossHealth;
        private BossHealthBar _bar;
        private int _lastHealth = -1;
        private int _lastMax = -1;
        private bool _inBattle;

        private void Awake() => _bar = GetComponent<BossHealthBar>();

        // Connected to the area's battle-start event, after the entrance cinematic.
        public void BeginBattle()
        {
            if (_bossHealth == null || _bossHealth.IsDead) return;
            _inBattle = true;
            _lastMax = _bossHealth.MaxHealth;
            _lastHealth = _bossHealth.CurrentHealth;
            _bar.Initialize(_lastMax, _lastHealth);
            _bar.Show();
        }

        public void EndBattle()
        {
            _inBattle = false;
            if (_bar != null) _bar.Hide();
        }

        private void LateUpdate()
        {
            if (!_inBattle) return;
            if (_bossHealth == null || !_bossHealth.gameObject.activeInHierarchy)
            {
                EndBattle();
                return;
            }
            if (_lastMax != _bossHealth.MaxHealth)
            {
                _lastMax = _bossHealth.MaxHealth;
                _bar.Initialize(_lastMax, _bossHealth.CurrentHealth);
            }
            if (_lastHealth != _bossHealth.CurrentHealth)
            {
                _lastHealth = _bossHealth.CurrentHealth;
                _bar.SetHealth(_lastHealth);
            }
            if (_bossHealth.IsDead) EndBattle();
        }

        private void OnDisable() => EndBattle();

        [ContextMenu("Test/Show Boss Health (Play Mode)")]
        private void TestShow()
        {
            if (Application.isPlaying) BeginBattle();
        }
    }
}
