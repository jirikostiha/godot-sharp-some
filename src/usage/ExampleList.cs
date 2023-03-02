using System.Linq;
using Godot;

public sealed partial class ExampleList : Godot.Control
{
	public static readonly Color LightBack = new Color("white");
	public static readonly Color DarkBack = new Color("black");
	public static readonly Color LightStroke = new Color("white");
	public static readonly Color DarkStroke = new Color("black");
	public static readonly Color LightRegion = new Color("lightgray");
	public static readonly Color DarkRegion = new Color("darkgray");

	public bool AnimateAll { get; set; }

	public bool InvertAll { get; set; }

	public void _on_AnimateAll_pressed()
	{
		AnimateAll = !AnimateAll;
		var nodes = GetTree().GetNodesInGroup("ExampleContents").OfType<ExampleNodeBase>();
		foreach (var node in nodes)
			node.Animate = AnimateAll;
	}

	public void _on_InvertAll_pressed()
	{
		InvertAll = !InvertAll;
		if (!InvertAll)
		{
			var nodes = GetTree().GetNodesInGroup("ExampleContents").OfType<ExampleNodeBase>();
			foreach (var node in nodes)
			{
				node.Color = LightBack;
				node.LineColor = DarkStroke;
				node.AreaColor = LightRegion;
				node.TextColor = DarkStroke;
			}
		}
		else
		{
			var nodes = GetTree().GetNodesInGroup("ExampleContents").OfType<ExampleNodeBase>();
			foreach (var node in nodes)
			{
				node.Color = DarkBack;
				node.LineColor = LightStroke;
				node.AreaColor = DarkRegion;
				node.TextColor = LightStroke;
			}
		}
	}
}
