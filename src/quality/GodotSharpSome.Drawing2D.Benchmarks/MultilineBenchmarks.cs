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
    private readonly List<(string Key, IStraightLineAppender Pen)> _customPalette = new(7) {
        new ("solid", new SolidLine()), new ("dashed", new DashedLine())};

    [Benchmark]
    public Multiline CreateDefault()
    {
        return new Multiline();
    }

    [Benchmark]
    public Multiline CreateDefaultWithList()
    {
        return new Multiline(_lines);
    }

    [Benchmark]
    public Multiline CreateDefaultWithListAndPalette()
    {
        return new Multiline(_lines, _customPalette);
    }

    [Benchmark]
    public Multiline CreateDefaultFourLineTypes()
    {
        return Multiline.FourLineTypes();
    }

    [Benchmark]
    public Multiline CreateCustomFourLineTypes()
    {
        return Multiline.FourLineTypes(6, 8);
    }
}