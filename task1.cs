using System;
using System.Threading;
using System.Threading.Tasks;

int[] numbers = new int[1000];
int max = 0, min = 0;
double avg = 0;

ManualResetEvent mre = new ManualResetEvent(false);

Task generator = Task.Run(() =>
{
    Console.WriteLine("Генерація чисел");
    Random rnd = new Random();
    for (int i = 0; i < 1000; i++)
        numbers[i] = rnd.Next(0, 5000);
    Console.WriteLine("Генерацію завершено");
    mre.Set();
});

Task t1 = Task.Run(() =>
{
    Console.WriteLine("Потік 1 очікує...");
    mre.WaitOne();
    Console.WriteLine("Потік 1 шукає максимум");
    max = numbers[0];
    foreach (int n in numbers)
        if (n > max) max = n;
    Console.WriteLine("Максимум: " + max);
});

Task t2 = Task.Run(() =>
{
    Console.WriteLine("Потік 2 очікує");
    mre.WaitOne();
    Console.WriteLine("Потік 2 шукає мінімум");
    min = numbers[0];
    foreach (int n in numbers)
        if (n < min) min = n;
    Console.WriteLine("Мінімум: " + min);
});

Task t3 = Task.Run(() =>
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

Task.WaitAll(generator, t1, t2, t3);
