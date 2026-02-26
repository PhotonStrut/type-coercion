using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using TypeCoercion;
using System;

namespace TypeCoercion.Benchmarks;

[MemoryDiagnoser]
public class CoercionBenchmarks
{
    private readonly TypeCoercionOptions _options = TypeCoercionOptions.Default;

    // --- Guid Coercion ---
    
    [Benchmark(Baseline = true)]
    public Guid Native_Guid_Parse_Valid()
    {
        return Guid.Parse("d3b07384-d113-40e1-a0a6-15ec83d6a2f9");
    }

    [Benchmark]
    public Guid TypeCoercion_Guid_Coerce_Valid()
    {
        return global::TypeCoercion.TypeCoercer.Coerce<Guid>("d3b07384-d113-40e1-a0a6-15ec83d6a2f9", _options);
    }

    [Benchmark]
    public bool Native_Guid_TryParse_Invalid()
    {
        return Guid.TryParse("not-a-guid", out _);
    }

    [Benchmark]
    public bool TypeCoercion_Guid_TryCoerce_Invalid()
    {
        return global::TypeCoercion.TypeCoercer.TryCoerce<Guid>("not-a-guid", _options).Success;
    }

    // --- DateTime Coercion ---

    [Benchmark]
    public DateTime Native_DateTime_Parse_Valid()
    {
        return DateTime.Parse("2026-03-15T10:30:00");
    }

    [Benchmark]
    public DateTime TypeCoercion_DateTime_Coerce_Valid()
    {
        return global::TypeCoercion.TypeCoercer.Coerce<DateTime>("2026-03-15T10:30:00", _options);
    }

    [Benchmark]
    public bool Native_DateTime_TryParse_Invalid()
    {
        return DateTime.TryParse("not-a-date", out _);
    }

    [Benchmark]
    public bool TypeCoercion_DateTime_TryCoerce_Invalid()
    {
        return global::TypeCoercion.TypeCoercer.TryCoerce<DateTime>("not-a-date", _options).Success;
    }

    // --- Fallback Coercion ---

    [Benchmark]
    public int Native_Convert_ChangeType_Valid()
    {
        return (int)Convert.ChangeType("123", typeof(int));
    }

    [Benchmark]
    public int TypeCoercion_Fallback_Coerce_Valid()
    {
        return global::TypeCoercion.TypeCoercer.Coerce<int>("123", _options);
    }

    [Benchmark]
    public bool TypeCoercion_Fallback_TryCoerce_Invalid()
    {
        return global::TypeCoercion.TypeCoercer.TryCoerce<int>("not-a-number", _options).Success;
    }
}
