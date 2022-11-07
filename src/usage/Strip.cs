using Godot;

public class Strip : Control
{
    //public static PackedScene CellScene = GD.Load<PackedScene>("res://Cell.tscn");

    private NodePath _groupNameLabelPath = "Group/Name";
    private NodePath _cellsPath = "Group/Cells";
    private static Label _groupNameLabel;
    private static HBoxContainer _cells;

    

    public string GroupName { get => _groupNameLabel.Text; set => _groupNameLabel.Text = value; }

    public HBoxContainer Cells => _cells;

    public override void _Ready()
    {
        base._Ready();
        //Color = Color.ColorN("white");
        _groupNameLabel = GetNode<Label>(_groupNameLabelPath);
        _cells = GetNode<HBoxContainer>(_cellsPath);
    }
}
