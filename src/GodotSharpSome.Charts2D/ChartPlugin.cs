#if TOOLS
using Godot;
using GodotSharpSome.Charts2D;
using System;

[Tool]
public class ChartPlugin : EditorPlugin
{
    public override string GetPluginName()
    {
        return nameof(Chart);
    }

    public override void _EnterTree()
    {
        GD.Print($"{nameof(ChartPlugin)} loaded");

        var script = GD.Load<Script>("addons/Chart/Chart.cs");

        AddCustomType(nameof(Chart), nameof(CoordinateSystem), script, null);
    }

    public override void _ExitTree()
    {
        RemoveCustomType(nameof(Chart));
    }
}
#endif
