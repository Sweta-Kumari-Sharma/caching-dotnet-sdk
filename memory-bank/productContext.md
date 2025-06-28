# Product Context

## Problem Space

The problem is that while .NET has some built-in caching capabilities, they can be complex to configure or may not offer all the features a developer needs out-of-the-box. A simple, lightweight, and easy-to-use caching library can significantly improve application performance by reducing the need to repeatedly fetch or compute data.

## User Goals

- **For the developer (us):** To gain experience with concurrent programming, memory management, and creating a high-performance .NET library.
- **For the end-user (of the SDK):** To have a simple and efficient way to cache data within their applications, improving performance with minimal setup.

## High-Level Functionality

The caching library will provide the following core features:
- **In-memory caching:** Store and retrieve objects from memory.
- **Expiration policies:** Set absolute or sliding expiration times for cached items.
- **Capacity limits:** Limit the number of items in the cache.
- **Eviction policies:** Define how items are removed when the cache is full (e.g., LRU - Least Recently Used).
- **Thread-safe operations:** Ensure the cache can be safely used in multi-threaded applications.
