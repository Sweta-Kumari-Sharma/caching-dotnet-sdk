using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace CacheKit
{
    public class Cache<TKey, TValue> : ICache<TKey, TValue>, IDisposable where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, CacheItem<TValue>> _items;
        private readonly Timer _cleanupTimer;
        private readonly int _capacity;
        private readonly object _lock = new object();
        private readonly CacheStatistics _statistics;
        private readonly IEvictionStrategy<TKey> _evictionStrategy;

        public event EventHandler<CacheEventArgs<TKey>>? ItemAdded;
        public event EventHandler<CacheEventArgs<TKey>>? ItemRemoved;
        public event EventHandler<CacheEventArgs<TKey>>? ItemEvicted;

        public Cache(int capacity = 1000, EvictionPolicy evictionPolicy = EvictionPolicy.LeastRecentlyUsed)
        {
            _capacity = capacity;
            _items = new ConcurrentDictionary<TKey, CacheItem<TValue>>();
            _statistics = new CacheStatistics();
            _cleanupTimer = new Timer(Cleanup, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            _evictionStrategy = evictionPolicy switch
            {
                EvictionPolicy.LeastFrequentlyUsed => new LfuEvictionStrategy<TKey>(),
                _ => new LruEvictionStrategy<TKey>(),
            };
        }

        public TValue? Get(TKey key)
        {
            if (TryGetValue(key, out TValue? value))
            {
                return value;
            }
            return default;
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            if (_items.TryGetValue(key, out CacheItem<TValue>? item))
            {
                if (!item.IsExpired())
                {
                    lock (_lock)
                    {
                        _evictionStrategy.OnAccess(key);
                    }
                    item.LastAccessed = DateTimeOffset.UtcNow;
                    value = item.Value;
                    _statistics.Hits++;
                    return true;
                }
                else
                {
                    Remove(key);
                }
            }

            _statistics.Misses++;
            value = default;
            return false;
        }

        public TValue? GetOrAdd(TKey key, Func<TKey, TValue> valueFactory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            if (TryGetValue(key, out TValue? value))
            {
                return value;
            }

            lock (_lock)
            {
                // Double-check locking
                if (TryGetValue(key, out value))
                {
                    return value;
                }

                var newValue = valueFactory(key);
                Set(key, newValue, absoluteExpiration, slidingExpiration);
                return newValue;
            }
        }

        public void Set(TKey key, TValue value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            if (_items.Count >= _capacity)
            {
                Evict();
            }
            var item = new CacheItem<TValue>(value, absoluteExpiration, slidingExpiration);
            _items[key] = item;

            lock (_lock)
            {
                _evictionStrategy.Add(key);
            }
            OnItemAdded(key);
            _statistics.Additions++;
        }

        public void Remove(TKey key)
        {
            if (_items.TryRemove(key, out _))
            {
                lock (_lock)
                {
                    _evictionStrategy.Remove(key);
                }
                OnItemRemoved(key);
                _statistics.Removals++;
            }
        }

        public int Count => _items.Count;

        public void Clear()
        {
            _items.Clear();
            lock (_lock)
            {
                // This is a bit of a hack, but it's the easiest way to clear the strategies
                if (_evictionStrategy is LruEvictionStrategy<TKey> lru)
                {
                    lru.GetType().GetField("_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(lru, new LinkedList<TKey>());
                    lru.GetType().GetField("_map", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(lru, new Dictionary<TKey, LinkedListNode<TKey>>());
                }
                else if (_evictionStrategy is LfuEvictionStrategy<TKey> lfu)
                {
                    lfu.GetType().GetField("_frequencies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(lfu, new Dictionary<TKey, int>());
                }
            }
        }

        private void Cleanup(object? state)
        {
            foreach (var key in _items.Keys)
            {
                if (_items.TryGetValue(key, out CacheItem<TValue>? item) && item.IsExpired())
                {
                    Remove(key);
                }
            }
        }

        private void Evict()
        {
            lock (_lock)
            {
                var keyToEvict = _evictionStrategy.GetKeyToEvict();
                if (keyToEvict != null)
                {
                    Remove(keyToEvict);
                    OnItemEvicted(keyToEvict);
                    _statistics.Evictions++;
                }
            }
        }

        protected virtual void OnItemAdded(TKey key)
        {
            ItemAdded?.Invoke(this, new CacheEventArgs<TKey>(key));
        }

        protected virtual void OnItemRemoved(TKey key)
        {
            ItemRemoved?.Invoke(this, new CacheEventArgs<TKey>(key));
        }

        protected virtual void OnItemEvicted(TKey key)
        {
            ItemEvicted?.Invoke(this, new CacheEventArgs<TKey>(key));
        }

        public CacheStatistics GetStatistics()
        {
            return _statistics;
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }
}
