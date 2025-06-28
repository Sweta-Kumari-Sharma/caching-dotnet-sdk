using System;

namespace CacheKit
{
    public class CacheEventArgs<TKey> : EventArgs where TKey : notnull
    {
        public TKey Key { get; }

        public CacheEventArgs(TKey key)
        {
            Key = key;
        }
    }
}
