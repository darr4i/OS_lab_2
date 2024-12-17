namespace SlabAllocator
{
    public class Extent
    {
        public int Size { get; }
        public bool IsFree { get; private set; }

        public Extent(int size)
        {
            Size = size;
            IsFree = true;
        }

        public void MarkUsed()
        {
            IsFree = false;
        }

        public void MarkFree()
        {
            IsFree = true;
        }
    }
}