# CacheKit: A .NET Caching Library

CacheKit is a feature-rich .NET SDK that provides an easy-to-use, in-memory caching solution. It is designed to be lightweight, performant, and flexible, with a variety of features to suit different caching needs.

## Project Structure

This repository contains two main projects:

1.  **`CacheKit/`**: This is the **SDK project**. It is a .NET class library that contains all the core caching logic. This is the project that you would package into a NuGet package to be consumed by other applications.

2.  **`SampleApp/`**: This is a **demonstration project**. It is a simple console application that shows how to use the `CacheKit` SDK and demonstrates its various features in action.

## Features

*   **Configurable Eviction Policies**: Choose between Least Recently Used (LRU) and Least Frequently Used (LFU) eviction policies.
*   **Cache Events**: Subscribe to events for when items are added, removed, or evicted from the cache.
*   **Detailed Statistics**: Track cache performance with statistics for hits, misses, additions, removals, and evictions.
*   **Atomic `GetOrAdd` Operation**: A thread-safe method to get an item from the cache or add it if it doesn't exist in a single operation.
*   **Time-Based Expiration**: Set absolute or sliding expiration times for cache items.

## How to Run the Demo

To see the `CacheKit` SDK in action, you can run the `SampleApp` project. From the root directory, execute the following command in your terminal:

```bash
dotnet run --project SampleApp/SampleApp.csproj
