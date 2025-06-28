using System;

namespace CacheKit
{
    /// <summary>
    /// Represents a generic cache of key-value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface ICache<TKey, TValue> where TKey : notnull
    {
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key, or the default value for the type TValue if the key is not found.</returns>
        TValue? Get(TKey key);

        /// <summary>
        /// Tries to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the cache contains an element with the specified key; otherwise, false.</returns>
        bool TryGetValue(TKey key, out TValue? value);

        /// <summary>
        /// Gets the value associated with the specified key, or adds it if it does not exist.
        /// </summary>
        /// <param name="key">The key of the value to get or add.</param>
        /// <param name="valueFactory">A function to create the value if it does not exist.</param>
        /// <param name="absoluteExpiration">The absolute expiration time for the cache item.</param>
        /// <param name="slidingExpiration">The sliding expiration time for the cache item.</param>
        /// <returns>The value associated with the specified key.</returns>
        TValue? GetOrAdd(TKey key, Func<TKey, TValue> valueFactory, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);

        /// <summary>
        /// Adds a key/value pair to the cache.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <param name="absoluteExpiration">The absolute expiration time for the cache item.</param>
        /// <param name="slidingExpiration">The sliding expiration time for the cache item.</param>
        void Set(TKey key, TValue value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);

        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        void Remove(TKey key);

        /// <summary>
        /// Gets the number of items contained in the cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Removes all keys and values from the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Occurs when an item is added to the cache.
        /// </summary>
        event EventHandler<CacheEventArgs<TKey>>? ItemAdded;

        /// <summary>
        /// Occurs when an item is removed from the cache.
        /// </summary>
        event EventHandler<CacheEventArgs<TKey>>? ItemRemoved;

        /// <summary>
        /// Occurs when an item is evicted from the cache.
        /// </summary>
        event EventHandler<CacheEventArgs<TKey>>? ItemEvicted;

        /// <summary>
        /// Gets the cache statistics.
        /// </summary>
        /// <returns>The cache statistics.</returns>
        CacheStatistics GetStatistics();
    }
}
