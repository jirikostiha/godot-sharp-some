using System.Collections.Generic;
using GodotSharpSome.Drawing2D.Tables;

public class Tables : ExampleNodeBase
{
    public override void _Draw()
    {
        var header = new List<string>() { "column 1", "column 2", "column 3"};
        var data = new List<IList<string>> {
            new List<string> { "cell 1,1", "cell 1,2", "cell 1,3" },
            new List<string> { "cell 2,1", "cell 2,2", "cell 2,3" },
            new List<string> { "cell 3,1", "cell 3,2", "cell 3,3" }};

        this.DrawTable(LeftBottom(1), header, data, GetFont(default), LineColor);
    }
}
