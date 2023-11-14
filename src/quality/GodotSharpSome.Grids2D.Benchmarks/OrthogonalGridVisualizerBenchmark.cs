using BenchmarkDotNet.Attributes;
using Godot;

namespace GodotSharpSome.Grids2D.Benchmarks;

[MemoryDiagnoser]
public class OrthogonalGridVisualizerBenchmark
{
    private readonly Control _control = new();
    private readonly OrthogonalGridVOptions _voptions = new();
    private readonly OrthogonalGridOptions _options = new();
    private OrthogonalGridVisualizer _vizer;

    [GlobalSetup]
    public void Setup()
    {
        _vizer = new(_control, _voptions);
    }

    [Benchmark]
    public OrthogonalGridVisualizer CreateDefault()
    {
        return new OrthogonalGridVisualizer(_control, _voptions);
    }

    [Benchmark]
    public void CreateDefaul()
    {
        _vizer.Draw(_options);
    }
}