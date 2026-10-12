using System.Collections.Generic;
using UnityEngine;

namespace Game.Traps
{
    public sealed class PhaseTrapPool : MonoBehaviour
    {
        [SerializeField] private PhaseTrapFlash _prefab;
        [SerializeField, Min(1)] private int _capacity = 4;
        private readonly List<PhaseTrapFlash> _items = new();
        public int ActiveCount
        {
            get
            {
                int count = 0;
                foreach (var item in _items)
                {
                    if (item != null && item.gameObject.activeSelf)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private void Awake()
        {
            if (_prefab == null) return;
            for (int i = 0; i < _capacity; i++)
            {
                var item = Instantiate(_prefab, transform);
                item.gameObject.SetActive(false);
                _items.Add(item);
            }
        }

        public PhaseTrapFlash Acquire()
        {
            foreach (var item in _items)
            {
                if (item != null && !item.gameObject.activeSelf)
                {
                    return item;
                }
            }

            return null;
        }

        public void ReleaseAll()
        {
            foreach (var item in _items)
            {
                if (item != null)
                {
                    item.Release();
                }
            }
        }

        private void OnDisable() => ReleaseAll();
    }
}