using Godot;
using GodotSharpSome.Grids2D;
using Xunit;
using NSubstitute;

namespace GodotSharpSome.Drawing2D.Tests
{
    public class OrthogonalGridVisualizerTest
    {
        [Fact(Skip = "todo")]
        public void Draw_0x0()
        {
            //var c = new Control();
            //var canvasMock = Substitute.For<Control>();
            //canvasMock.DrawMultiline(Arg.Any<Vector2[]>(), Arg.Any<Color>(), Arg.Any<float>());

            //var grid = new OrthogonalGridVisualizer(canvasMock);

            //grid.Draw(0,0);
        }

        [Fact(Skip = "todo")]
        public void Draw_1x1()
        {
            var grid = new OrthogonalGridVisualizer(null);

            grid.Draw(1, 1);
        }
    }
}