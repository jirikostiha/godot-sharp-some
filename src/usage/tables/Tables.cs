using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotSharpSome.Tables;

public partial class Tables : ExampleNodeBase
{
    private TableVisualizer _vizer;

    public Tables()
    {
        _vizer = new TableVisualizer(this);
    }

    public override void _Draw()
    {
        _vizer.DrawTable(GetRowData2().ToArray(), ThemeDB.FallbackFont, LineColor, LeftBottom(1));

        _vizer.DrawTable(Enumerable.Repeat(GetHeader2(), 1).Union(GetRowData2()).ToArray(), ThemeDB.FallbackFont, LineColor, LeftBottom(1));

        _vizer.DrawTable(GetRowData3().ToArray(), ThemeDB.FallbackFont, LineColor, new Vector2(140, 10));

        _vizer.DrawTable(GetRowData4().ToArray(), ThemeDB.FallbackFont, LineColor, new Vector2(300, 10));
    }

    private (string C1, string C2) GetHeader2() => ("Rank", "Name");

    private IEnumerable<(string C1, string C2)> GetRowData2() => GetRowData4().Select(x => (x.C1, x.C2));

    private IEnumerable<(string C1, string C2, string C3)> GetRowData3() => GetRowData4().Select(x => (x.C1, x.C2, x.C3));

    private IEnumerable<(string C1, string C2, string C3, string C4)> GetRowData4()
    {
        yield return ("1", "Melvil", "2h", "250");
        yield return ("2", "Overlord", "2h:30m", "210");
        yield return ("3", "Kira", "3h:10m", "95");
        yield return ("4", "Lukas", "3h:50m", "8");
    }
}
