using BenchmarkDotNet.Running;

namespace GodotSharpSome.Drawing2D.Benchmarks;

public static class Program
{
    public static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}