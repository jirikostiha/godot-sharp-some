using Godot;
using static Godot.Mathf;

namespace GodotSharpSome.Drawing2D.Examples;

public partial class Pendulum : ExampleNodeBase
{
	private Tween _tween;

	private float _state;

	private Multiline _ml = Multiline.FourLineTypes();

	public override void _Ready()
	{
		base._Ready();

		_tween = CreateTween().SetLoops();
		_tween.TweenMethod(new Callable(this, nameof(Interpolate)), 0f, 1f, 4f);
		_tween.TweenMethod(new Callable(this, nameof(Interpolate)), 1f, 0f, 4f);
	}

	public override void _Draw()
	{
		var center = Size / 2;
		var eyeCenter = new Vector2(center.X, center.Y * 0.5f);

		var points = _ml
			.Clear()
			.SetPen(0)
			// outer line
			.AppendLine(new(center.X, 10), new(10, Size.Y * 0.2f))
			.AppendLine(new(center.X * 0.6f, Size.Y * 0.85f))
			.AppendLine(new(center.X, Size.Y - 10))
			// inner line
			.AppendLine(new(center.X, Size.Y * 0.4f), new(center.X * 0.2f, Size.Y * 0.22f))
			.AppendLine(new(center.X * 0.7f, Size.Y * 0.84f))
			.AppendLine(new(center.X, Size.Y * 0.8f))
			.MirrorY(center.X)
			.Points();

		// Upper eye


		DrawMultiline(points, LineColor);
	}

	private void Interpolate(float value) => _state = value;
}
