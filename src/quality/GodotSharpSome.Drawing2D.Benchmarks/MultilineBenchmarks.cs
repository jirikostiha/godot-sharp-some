using BenchmarkDotNet.Attributes;
using Godot;

namespace GodotSharpSome.Drawing2D.Benchmarks;

[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
//[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
[MemoryDiagnoser]
public class MultilineBenchmarks
{
    private readonly List<Vector2> _lines = new(17);

    [Params(1000, 10_000, 100_000)]
    public int N { get; set; }

    [Benchmark]
    public Multiline? CreateDefault()
    {
        Multiline? ml = null;
        for (int i = 0; i < N; i++)
        {
            ml = new Multiline();
        }
        return ml;
    }

    [Benchmark]
    public Multiline? CreateDefaultWithList()
    {
        Multiline? ml = null;
        for (int i = 0; i < N; i++)
        {
            ml = new Multiline(_lines);
        }
        return ml;
    }

    [Benchmark]
    public Multiline? CreateDefaultFourLineTypes()
    {
        Multiline? ml = null;
        for (int i = 0; i < N; i++)
        {
            ml = Multiline.FourLineTypes();
        }
        return ml;
    }

    [Benchmark]
    public Multiline? CreateCustomFourLineTypes()
    {
        Multiline? ml = null;
        for (int i = 0; i < N; i++)
        {
            ml = Multiline.FourLineTypes(6, 8);
        }
        return ml;
    }

    [Benchmark]
    public Vector2[] AppendSolidLines()
    {
        var ml = new Multiline();
        for (int i = 0; i < N; i++)
        {
            ml.AppendLine(Vector2.Zero, Vector2.One);
            ml.AppendTriangle(Vector2.Zero, Vector2.One, Vector2.Up);
        }

        return ml.Points();
    }

    [Benchmark]
    public Vector2[] AppendDashedLines()
    {
        var ml = new Multiline(new DashedLine());
        for (int i = 0; i < N; i++)
        {
            ml.AppendLine(Vector2.Zero, Vector2.One);
            ml.AppendTriangle(Vector2.Zero, Vector2.One, Vector2.Up);
        }

        return ml.Points();
    }

    [Benchmark]
    public Vector2[] AppendDashedLinesDefaultAppender()
    {
        var ml = new Multiline(DashedLine.Default);
        for (int i = 0; i < N; i++)
        {
            ml.AppendLine(Vector2.Zero, Vector2.One);
            ml.AppendTriangle(Vector2.Zero, Vector2.One, Vector2.Up);
        }

        return ml.Points();
    }
}