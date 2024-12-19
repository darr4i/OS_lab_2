using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Collections;

namespace SlabAllocator
{
    public class AutoTester
    {
        private class AllocatedBlock
        {
            public IntPtr Pointer { get; set; }
            public int Size { get; set; }
            public byte[] Data { get; set; } = Array.Empty<byte>();
            public byte[] Checksum { get; set; } = Array.Empty<byte>();
        }

        private Random random = new Random();
        private List<AllocatedBlock> allocatedBlocks = new List<AllocatedBlock>();
        private MemoryAllocator allocator;

        public AutoTester(MemoryAllocator allocator)
        {
            this.allocator = allocator;
        }

        private byte[] ComputeChecksum(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        private byte[] FillRandomData(int size)
        {
            byte[] data = new byte[size];
            random.NextBytes(data);
            return data;
        }

        private bool VerifyBlock(AllocatedBlock block)
        {
            if (block == null || block.Pointer == IntPtr.Zero)
            {
                return false;
            }

            byte[] currentChecksum = ComputeChecksum(block.Data);
            return StructuralComparisons.StructuralEqualityComparer.Equals(block.Checksum, currentChecksum);
        }

        public void RunTests(int iterations)
        {
            Console.WriteLine("\n[INFO] Running automated tests...");

            for (int i = 0; i < iterations; i++)
            {
                int action = random.Next(3);
                switch (action)
                {
                    case 0: PerformAlloc(); break;
                    case 1: PerformRealloc(); break;
                    case 2: PerformFree(); break;
                }
            }

            Cleanup();
            Console.WriteLine("[INFO] Testing completed.");
        }

        private void PerformAlloc()
        {
            int size = random.Next(16, 512);
            IntPtr pointer = allocator.Allocate(size);
            

            if (pointer != IntPtr.Zero)
            {
                byte[] data = FillRandomData(size);
                byte[] checksum = ComputeChecksum(data);

                allocatedBlocks.Add(new AllocatedBlock
                {
                    Pointer = pointer,
                    Size = size,
                    Data = data,
                    Checksum = checksum
                });

                Console.WriteLine($"[ALLOC] Allocated {size} bytes.");
            }
            else
            {
                Console.WriteLine("[ERROR] Allocate failed to provide memory.");
            }
        }

                private void PerformRealloc()
        {
            if (allocatedBlocks.Count == 0) return;

            int index = random.Next(allocatedBlocks.Count);
            AllocatedBlock block = allocatedBlocks[index];

            if (!VerifyBlock(block))
            {
                Console.WriteLine("[ERROR] Data corruption detected before realloc.");
                return;
            }

            int newSize = random.Next(16, 2048);
            IntPtr newPointer = allocator.Allocate(newSize);

            if (newPointer == IntPtr.Zero)
            {
                Console.WriteLine("[ERROR] Failed to allocate memory for realloc.");
                return;
            }

            try
            {
                if (block.Data == null || block.Pointer == IntPtr.Zero)
                {
                    Console.WriteLine("[ERROR] Null reference during realloc!");
                    allocator.Deallocate(newPointer); // Звільняємо новий блок
                    return;
                }

                unsafe
                {
                    int copySize = Math.Min(block.Size, newSize);
                    if (copySize > 0)
                    {
                        Buffer.MemoryCopy((void*)block.Pointer, (void*)newPointer, newSize, copySize);
                    }
                }

                byte[] newData = new byte[newSize];
                Array.Copy(block.Data, newData, Math.Min(block.Size, newSize));
                block.Data = newData;
                block.Checksum = ComputeChecksum(newData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Memory copy failed: {ex.Message}");
                allocator.Deallocate(newPointer);
                return;
            }

            allocator.Deallocate(block.Pointer);

            block.Pointer = newPointer;
            block.Size = newSize;

            Console.WriteLine($"[REALLOC] Block resized to {newSize} bytes.");
        }


        private void PerformFree()
        {
            if (allocatedBlocks.Count == 0) return;

            int index = random.Next(allocatedBlocks.Count);
            AllocatedBlock block = allocatedBlocks[index];

            if (!VerifyBlock(block))
            {
                Console.WriteLine("[ERROR] Data corruption detected before free!");
                return;
            }

            allocator.Deallocate(block.Pointer);
            allocatedBlocks.RemoveAt(index);
            Console.WriteLine("[FREE] Block deallocated.");
        }

        private void Cleanup()
        {
            foreach (var block in allocatedBlocks)
            {
                if (!VerifyBlock(block))
                {
                    Console.WriteLine("[ERROR] Data corruption detected during cleanup!");
                }
                allocator.Deallocate(block.Pointer);
            }

            allocatedBlocks.Clear();
            Console.WriteLine("[INFO] All blocks deallocated.");
        }
    }

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
