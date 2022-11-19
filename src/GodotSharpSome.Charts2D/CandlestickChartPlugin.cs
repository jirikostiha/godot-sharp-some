#if TOOLS
using Godot;
using GodotSharpSome.Charts2D;
using System;

[Tool]
public class CandlestickChartPlugin : EditorPlugin
{
    public override string GetPluginName()
    {
        return nameof(CandlestickChart);
    }

    public override void _EnterTree()
    {
        GD.Print($"{nameof(CandlestickChartPlugin)} loaded");

        ResourceManager.Load();
        AddCustomType(nameof(CandlestickChart), nameof(CoordinateSystem), ResourceManager.LineChart, null);
    }

    public override void _ExitTree()
    {
        RemoveCustomType(nameof(CandlestickChart));
    }
}
#endif
