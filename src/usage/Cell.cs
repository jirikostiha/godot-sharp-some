using Godot;

public class Cell : Control
{
    private NodePath _titlePath = "Cell/Title";
    private NodePath _contentPath = "Cell/Content";
    private static Label _titleLabel;
    private static ColorRect _content;

    public string Title { get => _titleLabel.Text; set => _titleLabel.Text = value; }
    public ColorRect Content => _content;

    public override void _Ready()
    {
        base._Ready();
        _titleLabel = GetNode<Label>(_titlePath);
        _content = GetNode<ColorRect>(_contentPath);
    }
}
