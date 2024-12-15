using System;
using System.Collections;

namespace SlabAllocator
{
    public class Slab
    {
        private readonly int objectSize;
        private readonly int objectCount;
        private readonly BitArray bitmap;
        private readonly byte[] memory;

        public Slab(int objectSize, int pageSize)
        {
            this.objectSize = objectSize;
            this.objectCount = pageSize / objectSize;
            this.bitmap = new BitArray(objectCount, true);
            this.memory = new byte[pageSize];
        }

        public IntPtr Allocate()
        {
            for (int i = 0; i < objectCount; i++)
            {
                if (bitmap[i])
                {
                    bitmap[i] = false;
                    return new IntPtr(i * objectSize);
                }
            }
            return IntPtr.Zero; 
        }

        public void Deallocate(IntPtr ptr)
        {
            int index = ptr.ToInt32() / objectSize;
            bitmap[index] = true;
        }

        public bool IsEmpty()
        {
            foreach (bool bit in bitmap)
            {
                if (!bit) return false;
            }
            return true;
        }
        public int AllocatedSize => objectSize * objectCount;
    }
}
