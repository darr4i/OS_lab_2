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

            // Увеличиваем размер страницы до 16 объектов размером objectSize, если слишком быстро заполняются слейбы
            if (pageSize < objectSize * 16)
            {
                pageSize = objectSize * 16;
            }

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
            if (ptr == IntPtr.Zero) return; // Проверка на нулевой указатель
            int index = ptr.ToInt32() / objectSize;
            if (index >= 0 && index < objectCount)
            {
                bitmap[index] = true;
            }
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
