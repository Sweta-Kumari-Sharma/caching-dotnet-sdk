namespace CacheKit
{
    internal interface IEvictionStrategy<TKey> where TKey : notnull
    {
        void Add(TKey key);
        void Remove(TKey key);
        void OnAccess(TKey key);
        TKey? GetKeyToEvict();
    }
}
