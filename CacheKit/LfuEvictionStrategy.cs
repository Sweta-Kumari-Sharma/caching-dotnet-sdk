using System.Collections.Generic;
using System.Linq;

namespace CacheKit
{
    internal class LfuEvictionStrategy<TKey> : IEvictionStrategy<TKey> where TKey : notnull
    {
        private readonly Dictionary<TKey, int> _frequencies = new Dictionary<TKey, int>();

        public void Add(TKey key)
        {
            _frequencies[key] = 1;
        }

        public void Remove(TKey key)
        {
            _frequencies.Remove(key);
        }

        public void OnAccess(TKey key)
        {
            if (_frequencies.ContainsKey(key))
            {
                _frequencies[key]++;
            }
        }

        public TKey? GetKeyToEvict()
        {
            if (_frequencies.Count == 0)
            {
                return default;
            }

            var minFrequency = int.MaxValue;
            TKey? keyToEvict = default;

            foreach (var pair in _frequencies)
            {
                if (pair.Value < minFrequency)
                {
                    minFrequency = pair.Value;
                    keyToEvict = pair.Key;
                }
            }

            return keyToEvict;
        }
    }
}
