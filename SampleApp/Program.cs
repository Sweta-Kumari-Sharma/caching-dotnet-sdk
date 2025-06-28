using System;
using System.Threading;
using CacheKit;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CacheKit Demo");
            Console.WriteLine("================\n");

            // Create a cache with a capacity of 3 and LFU eviction policy
            var cache = new Cache<string, string>(3, EvictionPolicy.LeastFrequentlyUsed);

            // Subscribe to cache events
            cache.ItemAdded += (s, e) => Console.WriteLine($"Event: Item added - {e.Key}");
            cache.ItemRemoved += (s, e) => Console.WriteLine($"Event: Item removed - {e.Key}");
            cache.ItemEvicted += (s, e) => Console.WriteLine($"Event: Item evicted - {e.Key}");

            // Add some items to the cache
            Console.WriteLine("Adding 'apple', 'banana', 'orange' to the cache...");
            cache.Set("apple", "A sweet, red fruit");
            cache.Set("banana", "A long, yellow fruit");
            cache.Set("orange", "A round, orange fruit");

            // Access items to increase their frequency
            Console.WriteLine("\nAccessing 'apple' and 'orange' to make them more frequently used...");
            cache.Get("apple");
            cache.Get("orange");

            // Add another item, which should trigger eviction of the least frequently used item ('banana')
            Console.WriteLine("\nAdding 'grape' to the cache, which should evict 'banana'...");
            cache.Set("grape", "A small, purple fruit");

            // Demonstrate GetOrAdd
            Console.WriteLine("\nUsing GetOrAdd to retrieve 'grape' (should exist) and 'kiwi' (should be added)...");
            Console.WriteLine($"Value for 'grape': {cache.GetOrAdd("grape", (k) => "A small, green fruit")}");
            Console.WriteLine($"Value for 'kiwi': {cache.GetOrAdd("kiwi", (k) => "A small, brown fruit")}");

            // Demonstrate absolute expiration
            Console.WriteLine("\nAdding 'milk' with a 2-second absolute expiration...");
            cache.Set("milk", "A white, dairy drink", absoluteExpiration: TimeSpan.FromSeconds(2));
            Console.WriteLine($"Value for 'milk': {cache.Get("milk")}");
            Console.WriteLine("Waiting for 3 seconds...");
            Thread.Sleep(3000);
            Console.WriteLine($"Value for 'milk' after 3 seconds: {cache.Get("milk") ?? "null (expired)"}");

            // Print statistics
            var stats = cache.GetStatistics();
            Console.WriteLine("\nCache Statistics:");
            Console.WriteLine($"  Hits: {stats.Hits}");
            Console.WriteLine($"  Misses: {stats.Misses}");
            Console.WriteLine($"  Additions: {stats.Additions}");
            Console.WriteLine($"  Removals: {stats.Removals}");
            Console.WriteLine($"  Evictions: {stats.Evictions}");

            Console.WriteLine("\nDemo complete.");
        }
    }
}
