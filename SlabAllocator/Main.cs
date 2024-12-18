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

        // Автоматичне тестування
        Console.WriteLine("\n=== Автоматичне тестування алокатора пам'яті ===");
        AutoTester tester = new AutoTester(allocator);

        // Запуск 1000 випадкових операцій
        tester.RunTests(100);

        Console.WriteLine("Тестування завершено. Стан пам'яті:");
        allocator.MemShow();

        Console.WriteLine("Завершення роботи програми.");
    }
}
