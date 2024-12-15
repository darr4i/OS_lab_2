using System;
using System.Collections.Generic;

namespace SlabAllocator
{
    public class MemoryAllocator
    {
        private readonly List<MemoryCache> caches;
        private readonly int pageSize = 4096; 
        private readonly int arenaSize = 4096 * 16; 

        public MemoryAllocator()
        {
            caches = new List<MemoryCache>();
            InitializeCaches();
        }

        private void InitializeCaches()
        {
            int[] objectSizes = { 16, 32, 64, 128, 256, 512, 1024 };
            foreach (var size in objectSizes)
            {
                caches.Add(new MemoryCache(size, pageSize));
            }
        }

        public IntPtr Allocate(int size)
        {
            foreach (var cache in caches)
            {
                if (cache.CanAllocate(size))
                {
                    return cache.Allocate();
                }
            }
            throw new OutOfMemoryException("Не вдалося виділити пам'ять");
        }

        public void Deallocate(IntPtr ptr)
        {
            foreach (var cache in caches)
            {
                if (cache.Contains(ptr))
                {
                    cache.Deallocate(ptr);
                    return;
                }
            }
            throw new InvalidOperationException("Спроба звільнити недійсний вказівник");
        }

        public void MemShow()
        {
            foreach (var cache in caches)
            {
                cache.ShowStatus();
            }
        }
    }
}
