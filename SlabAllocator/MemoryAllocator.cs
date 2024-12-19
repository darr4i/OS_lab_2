using System;
using System.Collections.Generic;

namespace SlabAllocator
{
    public class MemoryAllocator
    {
        private const int PageSize = 4096; 
        private Dictionary<int, List<Slab>> slabCache; 

        public MemoryAllocator()
        {
            slabCache = new Dictionary<int, List<Slab>>();
        }

        public IntPtr Allocate(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "Розмір має бути більше 0");

            int slabSize = GetNearestPowerOfTwo(size);

            if (!slabCache.ContainsKey(slabSize))
            {
                AddNewSlab(slabSize);
            }

            foreach (var slab in slabCache[slabSize])
            {
                IntPtr ptr = slab.Allocate();
                if (ptr != IntPtr.Zero)
                {
                    return ptr;
                }
            }

            Console.WriteLine($"[INFO] Усі наявні slabs для розміру {slabSize} заповнені. Створюємо новий.");
            AddNewSlab(slabSize);
            return slabCache[slabSize][^1].Allocate();
        }

        public void Deallocate(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return;

            foreach (var kvp in slabCache)
            {
                foreach (var slab in kvp.Value)
                {
                    slab.Deallocate(ptr);
                }
            }

            RemoveEmptySlabs();
        }

        public void MemShow()
        {
            Console.WriteLine("Стан пам'яті:");

            foreach (var kvp in slabCache)
            {
                int objectSize = kvp.Key;
                int slabCount = kvp.Value.Count;

                Console.WriteLine($"Кеш об'єктів розміром {objectSize} байт: {slabCount} slabs.");
                foreach (var slab in kvp.Value)
                {
                    Console.WriteLine($"   Slab (Allocated: {slab.AllocatedSize}, Empty: {slab.IsEmpty()}, Used: {slab.UsedCount})");
                }
            }
        }

        private void AddNewSlab(int objectSize)
        {
            if (!slabCache.ContainsKey(objectSize))
            {
                slabCache[objectSize] = new List<Slab>();
            }
            slabCache[objectSize].Add(new Slab(objectSize, PageSize));
        }

        public void RemoveEmptySlabs()
        {
            foreach (var kvp in slabCache)
            {
                kvp.Value.RemoveAll(slab => slab.IsEmpty());
            }
        }

        private int GetNearestPowerOfTwo(int size)
        {
            int power = 1;
            while (power < size) power *= 2;
            return power;
        }
    }

}
