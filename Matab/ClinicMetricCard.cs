using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Matab
{
    // Reusable, double-buffered metric surface. Purely visual; no database access.
    internal sealed class ClinicMetricCard : Panel
    {
        public ClinicMetricCard()
        {
            DoubleBuffered = true;
            Padding = new Padding(10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var accent = new SolidBrush(Color.FromArgb(0, 145, 159)))
                e.Graphics.FillRectangle(accent, 0, 0, 5, Height);
            using (var pen = new Pen(Color.FromArgb(224, 235, 242)))
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}
