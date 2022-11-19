#if TOOLS
using Godot;
using GodotSharpSome.Charts2D;
using System;

//https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html

[Tool]
public class CoordinateSystemPlugin : EditorPlugin
{
    public override string GetPluginName()
    {
        return nameof(CoordinateSystem);
    }

    public override void _EnterTree()
    {
        GD.Print($"{nameof(CoordinateSystemPlugin)} loaded");

        ResourceManager.Load();
        AddCustomType(nameof(CoordinateSystem), nameof(ColorRect), ResourceManager.CoordinateSystem, null);
    }

    public override void _ExitTree()
    {
        RemoveCustomType(nameof(CoordinateSystem));
    }
}
#endif
