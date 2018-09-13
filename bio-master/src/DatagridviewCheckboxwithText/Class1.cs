using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class DataGridViewCheckAndTextColumn : DataGridViewCheckBoxColumn
    {
        public DataGridViewCheckAndTextColumn()
        {
            this.CellTemplate = new DataGridViewCheckAndTextCell();
        }
    }

    public class DataGridViewCheckAndTextCell : DataGridViewCheckBoxCell
    {
        public DataGridViewCheckAndTextCell()
        {
            this.Enabled = true;
        }

        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                this.ReadOnly = !enabled;
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private System.Drawing.Color color;
        public System.Drawing.Color Color
        {
            get
            {
                return color;
            }
            set { color = value; }
        }

        private System.Drawing.Font font;
        public System.Drawing.Font Font
        {
            get { return font; }
            set { font = value; }
        }
    protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
    int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue,
    string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
    DataGridViewPaintParts paintParts)
    {
        base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value,
            formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

        if (this.Font == null)
            this.Font = cellStyle.Font;

        if (!this.Enabled)
            this.Color = Color.Gray;
        else if (this.Color.IsEmpty)
            this.Color = cellStyle.ForeColor;

        CheckBoxState state;
        bool val = this.Value == null || !Convert.ToBoolean(this.Value) ? false : Convert.ToBoolean(this.Value);
        if (this.enabled && val)
            state = CheckBoxState.CheckedNormal;
        else if (this.enabled && !val)
            state = CheckBoxState.UncheckedNormal;
        else if (!this.enabled && val)
            state = CheckBoxState.CheckedDisabled;
        else
            state = CheckBoxState.UncheckedDisabled;

        Point loc = new Point(cellBounds.X + 2, cellBounds.Y + 2);
        CheckBoxRenderer.DrawCheckBox(graphics, loc, state);

        Rectangle contentBounds = this.GetContentBounds(rowIndex);
        Point stringLocation = new Point();
        stringLocation.Y = cellBounds.Y + 2;
        stringLocation.X = cellBounds.X + contentBounds.Right + 2;
        graphics.DrawString(this.Text, this.Font, new SolidBrush(this.Color), stringLocation);
    }


}



