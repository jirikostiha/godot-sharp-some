namespace GodotSharpSome.Tables
{
    using System;
    using System.Collections.Generic;
    using Godot;
    using GodotSharpSome.Grids2D;

    /// <summary>
    ///
    /// </summary>
    public partial class DrawnTable : ColorRect
    {
        private TableVisualizer _vizer;

        public DrawnTable()
            : this(new TableVOptions())
        { }

        public DrawnTable(TableVOptions options)
        {
            _vizer = new TableVisualizer(this);
        }

        public TableVOptions VOptions { get => _vizer.VOptions; }

        public IList<string[]> Data { get; set; }

        #region export
        //[Export] public string XName { get => _options.XAxis.Name ?? string.Empty; set => _options.XAxis.Name = value; }
        #endregion

        public override void _Draw()
        {
            _vizer.Draw(Data);

            base._Draw();
        }
    }
}
