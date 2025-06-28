namespace CacheKit
{
    public class CacheStatistics
    {
        public long Hits { get; internal set; }
        public long Misses { get; internal set; }
        public long Evictions { get; internal set; }
        public long Additions { get; internal set; }
        public long Removals { get; internal set; }

        internal void Reset()
        {
            Hits = 0;
            Misses = 0;
            Evictions = 0;
            Additions = 0;
            Removals = 0;
        }
    }
}
