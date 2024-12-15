using System;
using SlabAllocator;

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
