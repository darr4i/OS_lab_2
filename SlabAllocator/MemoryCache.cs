using System;
using System.Collections.Generic;

namespace SlabAllocator
{
    public class MemoryCache
    {
        private readonly int objectSize;
        private readonly Queue<Slab> slabs;

        public MemoryCache(int objectSize, int pageSize)
        {
            this.objectSize = objectSize;
            this.slabs = new Queue<Slab>();
            AddNewSlab(pageSize);
        }

        public bool CanAllocate(int size)
        {
            return size <= objectSize;
        }

        public IntPtr Allocate()
        {
            foreach (var slab in slabs)
            {
                IntPtr ptr = slab.Allocate();
                if (ptr != IntPtr.Zero)
                {
                    return ptr;
                }
            }

            AddNewSlab(objectSize * 8);
            return slabs.Peek().Allocate();
        }

        public void Deallocate(IntPtr ptr)
        {
            foreach (var slab in slabs)
            {
                slab.Deallocate(ptr);
            }
        }
    public bool Contains(IntPtr ptr)
    {
        foreach (var slab in slabs)
        {
             if (!slab.IsEmpty() && ptr.ToInt32() >= 0 && ptr.ToInt32() < slab.AllocatedSize)
            {
            return true;
            }
        }
        return false;
    }


        public void ShowStatus()
        {
            Console.WriteLine($"Кеш об'єктів розміром {objectSize} байт: {slabs.Count} slabs.");
        }

        private void AddNewSlab(int pageSize)
        {
            slabs.Enqueue(new Slab(objectSize, pageSize));
        }
        
        
    }
    
}