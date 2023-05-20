using System.Diagnostics;
using System.Text;

static void Benchmark(Action act, int iterations, string name)
{
    GC.Collect();
    act.Invoke();
    Stopwatch sw = Stopwatch.StartNew();
    var heap = GC.GetAllocatedBytesForCurrentThread();
    for (int i = 0; i < iterations; i++)
    {
        act.Invoke();
    }
    heap = GC.GetAllocatedBytesForCurrentThread() - heap;
    sw.Stop();
    Console.WriteLine($"Results for {name}:");
    Console.WriteLine(sw.ElapsedMilliseconds / iterations + "ms");
    Console.WriteLine(heap / iterations + "B");
}

var data = Enumerable.Range(1, 20000)
            .Select(i => DateTime.Now.AddDays(i).ToShortDateString())
            .ToList();

Console.WriteLine("Starting Benchmarks");
Benchmark(() => MethodStringJoin(data), 100, nameof(MethodStringJoin));
Benchmark(() => MethodAggregateDumb(data), 100, nameof(MethodAggregateDumb));
Benchmark(() => MethodAggregateGood(data), 100, nameof(MethodAggregateGood));
Console.ReadLine();

static void MethodStringJoin(IEnumerable<string> data)
{
    var output = string.Join(",", data);
    Debug.WriteLine(output);
}

static void MethodAggregateDumb(IEnumerable<string> data)
{
    var output = data.Aggregate((current, next) => $"{current},{next}").ToString();
    Debug.WriteLine(output);
}

static void MethodAggregateGood(IEnumerable<string> data)
{
    var output = data.Aggregate(new StringBuilder(), (current, next) => current.Append(",").Append(next)).ToString();
    Debug.WriteLine(output);
}
