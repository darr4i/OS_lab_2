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
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "Размер должен быть больше 0");

            int slabSize = GetNearestPowerOfTwo(size);

            if (!slabCache.ContainsKey(slabSize) || slabCache[slabSize].Count == 0)
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
        }

        public void MemShow()
        {
            Console.WriteLine("Состояние памяти:");

            foreach (var kvp in slabCache)
            {
                int objectSize = kvp.Key;
                int slabCount = kvp.Value.Count;

                Console.WriteLine($"Кеш объектов размером {objectSize} байт: {slabCount} slabs.");
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

        private int GetNearestPowerOfTwo(int size)
        {
            int power = 1;
            while (power < size) power *= 2;
            return power;
        }
    }
}
