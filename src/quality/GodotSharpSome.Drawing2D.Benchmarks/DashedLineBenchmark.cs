using BenchmarkDotNet.Attributes;
using Godot;

namespace GodotSharpSome.Drawing2D.Benchmarks;

[MemoryDiagnoser]
public class DashedLineBenchmark
{
    private readonly List<Vector2> _points = new(100);
    private readonly DashedLine _dashedLine = new();

    [Benchmark]
    public DashedLine CreateDefault()
    {
        return new DashedLine();
    }

    [Benchmark]
    public DashedLine CreateCustom()
    {
        return new DashedLine(5.5f, 4f);
    }

    [Benchmark]
    public void CreateDefaultWithListAndPalette()
    {
        _points.Clear();
        _dashedLine.AppendLine(_points, Vector2.Zero, new (0, 100));
    }

    [Benchmark]
    public void CreateDefaultFourLineTypes()
    {
        _points.Clear();
        DashedLine.AppendLine(_points, Vector2.Zero, new(0, 100), 12, 8);
    }
}