using System.Collections.Generic;

namespace CacheKit
{
    internal class LruEvictionStrategy<TKey> : IEvictionStrategy<TKey> where TKey : notnull
    {
        private readonly LinkedList<TKey> _list = new LinkedList<TKey>();
        private readonly Dictionary<TKey, LinkedListNode<TKey>> _map = new Dictionary<TKey, LinkedListNode<TKey>>();

        public void Add(TKey key)
        {
            var node = new LinkedListNode<TKey>(key);
            _list.AddLast(node);
            _map[key] = node;
        }

        public void Remove(TKey key)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                _map.Remove(key);
            }
        }

        public void OnAccess(TKey key)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                _list.AddLast(node);
            }
        }

        public TKey? GetKeyToEvict()
        {
            var first = _list.First;
            return first != null ? first.Value : default;
        }
    }
}
