using System;

namespace CacheKit
{
    internal class CacheItem<TValue>
    {
        public TValue Value { get; }
        public DateTimeOffset? AbsoluteExpiration { get; }
        public TimeSpan? SlidingExpiration { get; }
        public DateTimeOffset LastAccessed { get; set; }

        public CacheItem(TValue value, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            Value = value;
            if (absoluteExpiration.HasValue)
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow + absoluteExpiration.Value;
            }
            SlidingExpiration = slidingExpiration;
            LastAccessed = DateTimeOffset.UtcNow;
        }

        public bool IsExpired()
        {
            if (AbsoluteExpiration.HasValue && AbsoluteExpiration.Value <= DateTimeOffset.UtcNow)
            {
                return true;
            }

            if (SlidingExpiration.HasValue && (DateTimeOffset.UtcNow - LastAccessed) >= SlidingExpiration.Value)
            {
                return true;
            }

            return false;
        }
    }
}
