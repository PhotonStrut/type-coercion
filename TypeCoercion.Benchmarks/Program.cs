using BenchmarkDotNet.Running;

namespace TypeCoercion.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<CoercionBenchmarks>();
    }
}
