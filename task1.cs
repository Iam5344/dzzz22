using System;
using System.Threading;

int[] numbers = new int[1000];
int max = 0, min = 0;
double avg = 0;

ManualResetEvent mre = new ManualResetEvent(false);

Thread generator = new Thread(() =>
{
    Console.WriteLine("Генерація чисел...");
    Random rnd = new Random();
    for (int i = 0; i < 1000; i++)
        numbers[i] = rnd.Next(0, 5000);
    Console.WriteLine("Генерацію завершено");
    mre.Set();
});

Thread t1 = new Thread(() =>
{
    Console.WriteLine("Потік 1 очікує...");
    mre.WaitOne();
    Console.WriteLine("Потік 1 шукає максимум");
    max = numbers[0];
    foreach (int n in numbers)
        if (n > max) max = n;
    Console.WriteLine("Максимум: " + max);
});

Thread t2 = new Thread(() =>
{
    Console.WriteLine("Потік 2 очікує");
    mre.WaitOne();
    Console.WriteLine("Потік 2 шукає мінімум");
    min = numbers[0];
    foreach (int n in numbers)
        if (n < min) min = n;
    Console.WriteLine("Мінімум: " + min);
});

Thread t3 = new Thread(() =>
{
    Console.WriteLine("Потік 3 очікує");
    mre.WaitOne();
    Console.WriteLine("Потік 3 обчислює середнє");
    long sum = 0;
    foreach (int n in numbers)
        sum += n;
    avg = (double)sum / numbers.Length;
    Console.WriteLine("Середнє: " + avg);
});

t1.Start();
t2.Start();
t3.Start();
generator.Start();

generator.Join();
t1.Join();
t2.Join();
t3.Join();

