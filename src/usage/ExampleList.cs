using Godot;
using System.Linq;

public class ExampleList : Godot.Control
{
    private static readonly Color BackColor = Color.ColorN("white");

    public bool AnimateAll { get; set; }

    public void _on_AnimateAll_pressed()
    {
        AnimateAll = !AnimateAll;
        var nodes = GetTree().GetNodesInGroup("ExampleContents").OfType<ExampleNodeBase>();
        foreach (var node in nodes)
            node.Animate = AnimateAll;
    }
}
