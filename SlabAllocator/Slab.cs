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
                bitmap[i] = false; // Позначаємо блок як зайнятий
                return new IntPtr(i * objectSize);
            }
        }
        return IntPtr.Zero; // Усі блоки зайняті
    }
        public void Deallocate(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return; 
            int index = ptr.ToInt32() / objectSize;
            if (index >= 0 && index < objectCount)
            {
                bitmap[index] = true; // Блок позначається як вільний
            }
        }


    public bool IsEmpty()
    {
        foreach (bool bit in bitmap)
        {
            if (bit) return true; 
        }
        return false; 
    }

    public int AllocatedSize => objectSize * objectCount;

    public int UsedCount
    {
        get
        {
            int count = 0;
            foreach (bool bit in bitmap)
            {
                if (!bit) count++; // Рахуємо зайняті блоки
            }
            return count;
        }
    }
}

}
