#if TOOLS
using Godot;
using GodotSharpSome.Charts2D;
using System;

[Tool]
public class LineChartPlugin : EditorPlugin
{
    public override string GetPluginName()
    {
        return nameof(LineChart);
    }

    public override void _EnterTree()
    {
        GD.Print($"{nameof(LineChartPlugin)} loaded");

        ResourceManager.Load();
        AddCustomType(nameof(LineChart), nameof(CoordinateSystem), ResourceManager.LineChart, null);
    }

    public override void _ExitTree()
    {
        RemoveCustomType(nameof(LineChart));
    }
}
#endif
