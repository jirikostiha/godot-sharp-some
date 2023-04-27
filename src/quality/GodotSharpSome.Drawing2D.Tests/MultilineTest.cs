using Godot;

namespace GodotSharpSome.Drawing2D.Tests
{
    public class MultilineTest
    {
        [Fact]
        public void AppendLine_TwoPoints()
        {
            Assert.Equal(2, new Multiline().AppendLine(Vector2.Zero, Vector2.One).Points().Length);
        }
    }
}