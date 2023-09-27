#if TOOLS
using Godot;
using GodotSharpSome.Charts2D;
using System;

//https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html

[Tool]
public partial class CoordinateSystemPlugin : EditorPlugin
{
    public override string GetPluginName()
    {
        return nameof(CoordinateSystem);
    }

    public override void _EnterTree()
    {
        GD.Print($"{nameof(CoordinateSystemPlugin)} loaded");

        var script = GD.Load<Script>("addons/CoordinateSystem/CoordinateSystem.cs");

        AddCustomType(nameof(CoordinateSystem), nameof(ColorRect), script, null);
    }

    public override void _ExitTree()
    {
        RemoveCustomType(nameof(CoordinateSystem));
    }
}
#endif
