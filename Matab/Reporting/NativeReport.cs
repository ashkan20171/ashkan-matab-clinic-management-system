using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace Matab.Reporting
{
    // Standalone replacement for the legacy Stimulsoft preview calls.
    // It prints the currently displayed grid, or form values when no grid exists.
    internal sealed class NativeReport
    {
        private readonly Form source;
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();
        private string title = "گزارش مطب";
        private readonly List<string> lines = new List<string>();
        private int position;
        public NativeReport(Form form) { source = form; }
        public object this[string key] { get { return values.ContainsKey(key) ? values[key] : ""; } set { values[key] = Convert.ToString(value); } }
        public void Load(string path) { title = System.IO.Path.GetFileNameWithoutExtension(path); }
        public void Compile() { }
        public void ShowWithRibbonGUI()
        {
            lines.Clear();
            foreach (var item in values) lines.Add(item.Key + " : " + item.Value);
            DataGridView grid = FindControls(source).OfType<DataGridView>().FirstOrDefault(g => g.Visible && g.Columns.Count > 0);
            if (grid != null)
            {
                var cols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToArray();
                lines.Add(string.Join(" | ", cols.Select(c => c.HeaderText).ToArray()));
                foreach (DataGridViewRow row in grid.Rows)
                    if (!row.IsNewRow) lines.Add(string.Join(" | ", cols.Select(c => Convert.ToString(row.Cells[c.Index].FormattedValue)).ToArray()));
            }
            else if (values.Count == 0)
            {
                foreach (Control c in FindControls(source))
                    if (c is TextBoxBase || c is DateTimePicker || c is MaskedTextBox)
                        if (!string.IsNullOrWhiteSpace(c.Text)) lines.Add(c.Name + " : " + c.Text);
            }
            if (lines.Count == 0) { MessageBox.Show("داده‌ای برای چاپ در این فرم وجود ندارد.", "گزارش"); return; }
            position = 0;
            using (var document = new PrintDocument())
            {
                document.DocumentName = title;
                document.PrintPage += PrintPage;
                using (var dialog = new PrintPreviewDialog())
                {
                    dialog.Document = document;
                    dialog.Width = 1100;
                    dialog.Height = 750;
                    dialog.ShowDialog(source);
                }
            }
        }
        private static IEnumerable<Control> FindControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;
                foreach (var nested in FindControls(child)) yield return nested;
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            using (var font = new Font("Tahoma", 9))
            using (var header = new Font("Tahoma", 14, FontStyle.Bold))
            using (var format = new StringFormat { Alignment = StringAlignment.Far, FormatFlags = StringFormatFlags.DirectionRightToLeft })
            {
                float y = e.MarginBounds.Top;
                if (position == 0)
                {
                    e.Graphics.DrawString(title, header, Brushes.Black, new RectangleF(e.MarginBounds.Left, y, e.MarginBounds.Width, 35), format);
                    y += 45;
                }
                while (position < lines.Count && y + 34 < e.MarginBounds.Bottom)
                {
                    e.Graphics.DrawString(lines[position++], font, Brushes.Black, new RectangleF(e.MarginBounds.Left, y, e.MarginBounds.Width, 30), format);
                    y += 32;
                }
                e.HasMorePages = position < lines.Count;
            }
        }
    }
}
