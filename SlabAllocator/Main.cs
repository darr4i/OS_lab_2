using System;
using SlabAllocator;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Запуск slab-алокатора пам'яті...");

        MemoryAllocator allocator = new MemoryAllocator();

        var ptr1 = allocator.Allocate(128);
        var ptr2 = allocator.Allocate(64);

        Console.WriteLine("Стан пам'яті після виділення:");
        allocator.MemShow();

        allocator.Deallocate(ptr1);
        allocator.Deallocate(ptr2);

        Console.WriteLine("Стан пам'яті після звільнення:");
        allocator.MemShow();

       Console.WriteLine("\n=== Автоматичне тестування алокатора пам'яті ===");

        // Виділення блоків пам'яті
        List<IntPtr> allocatedPointers = new List<IntPtr>();
        for (int i = 0; i < 10; i++)
        {
            IntPtr ptr = allocator.Allocate(128);
            if (ptr != IntPtr.Zero)
            {
                Console.WriteLine($"[ALLOC] Виділено {128} байт за адресою {ptr}");
                allocatedPointers.Add(ptr);
            }
            else
            {
                Console.WriteLine("[ERROR] Помилка виділення пам'яті.");
            }
        }

                // Звільнення пам'яті
            foreach (var ptr in allocatedPointers)
        {
            allocator.Deallocate(ptr);
            Console.WriteLine($"[FREE] Звільнено пам'ять за адресою {ptr}");
        }

        // Видаляємо порожні Slabs
        allocator.MemShow(); // Показ стану до очищення
        allocator.RemoveEmptySlabs(); // Видаляємо порожні Slabs
        allocator.MemShow(); // Показ стану після очищення

    }

}
