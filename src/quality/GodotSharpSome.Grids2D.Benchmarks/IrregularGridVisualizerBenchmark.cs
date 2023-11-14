using BenchmarkDotNet.Attributes;
using Godot;

namespace GodotSharpSome.Grids2D.Benchmarks;

[MemoryDiagnoser]
public class IrregularGridVisualizerBenchmark
{
    private readonly Control _control = new();
    private readonly OrthogonalGridVOptions _voptions = new();
    private readonly IrregularGridOptions _options = new();
    private IrregularGridVisualizer _vizer;

    [GlobalSetup]
    public void Setup()
    {
        _vizer = new(_control, _voptions);
    }

    [Benchmark]
    public IrregularGridVisualizer CreateDefault()
    {
        return new IrregularGridVisualizer(_control, _voptions);
    }

    [Benchmark]
    public void Draw()
    {
        new IrregularGridVisualizer(_control, _voptions).Draw(_options);
    }
}