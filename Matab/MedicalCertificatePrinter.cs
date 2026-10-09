using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;

namespace Matab
{
    internal sealed class CertificatePrintModel
    {
        public string Patient, Reason, Location, Date, Phone;
        public int Days;
        public bool English;
    }

    internal static class MedicalCertificatePrinter
    {
        internal static PrintDocument Create(CertificatePrintModel model)
        {
            var doc = new PrintDocument();
            doc.DocumentName = model.English ? "Ashkan Matab - Medical Leave Certificate" : "اشکان مطب - گواهی استراحت";
            doc.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            doc.DefaultPageSettings.Margins = new Margins(35, 35, 35, 35);
            doc.PrintPage += (sender, e) => { Draw(e.Graphics, e.MarginBounds, model); e.HasMorePages = false; };
            return doc;
        }

        private static void Draw(Graphics g, Rectangle bounds, CertificatePrintModel m)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var navy = Color.FromArgb(18, 54, 80);
            var teal = Color.FromArgb(12, 138, 157);
            var light = Color.FromArgb(230, 246, 248);
            var x = bounds.Left; var y = bounds.Top; var w = bounds.Width;
            using (var border = new Pen(teal, 2)) g.DrawRoundedRectangleSafe(border, new Rectangle(x + 3, y + 3, w - 6, bounds.Height - 6), 16);
            using (var brush = new SolidBrush(navy)) g.FillRectangle(brush, x + 10, y + 12, w - 20, 108);
            using (var brush = new SolidBrush(teal)) g.FillRectangle(brush, x + 10, y + 117, w - 20, 7);
            using (var white = new SolidBrush(Color.White))
            using (var brand = new Font("Segoe UI", 22, FontStyle.Bold))
            using (var small = new Font("Segoe UI", 10))
            {
                g.DrawString("ASHKAN MATAB", brand, white, x + 34, y + 29);
                g.DrawString("CLINIC MANAGEMENT SYSTEM", small, white, x + 36, y + 75);
            }
            var rtl = !m.English;
            var flags = rtl ? StringFormatFlags.DirectionRightToLeft : 0;
            using (var format = new StringFormat { Alignment = StringAlignment.Center, FormatFlags = flags })
            using (var title = new Font(rtl ? "Tahoma" : "Segoe UI", 19, FontStyle.Bold))
            using (var ink = new SolidBrush(navy))
            {
                g.DrawString(rtl ? "گواهی استراحت پزشکی" : "MEDICAL LEAVE CERTIFICATE", title, ink,
                    new RectangleF(x + 30, y + 165, w - 60, 53), format);
            }
            using (var brush = new SolidBrush(light)) g.FillRectangle(brush, x + 36, y + 245, w - 72, 285);
            using (var ink = new SolidBrush(navy))
            using (var font = new Font(rtl ? "Tahoma" : "Segoe UI", 12))
            using (var fmt = new StringFormat { Alignment = rtl ? StringAlignment.Far : StringAlignment.Near, FormatFlags = flags })
            {
                var rect = new RectangleF(x + 60, y + 268, w - 120, 245);
                var content = rtl
                    ? "بدین وسیله گواهی می‌شود:\n\nنام بیمار: " + m.Patient + "\nتاریخ صدور: " + m.Date + "\nمدت استراحت: " + m.Days + " روز\nعلت مراجعه: " + (string.IsNullOrEmpty(m.Reason) ? "طبق معاینه پزشکی" : m.Reason) + "\n\nبا توجه به معاینه انجام‌شده، استراحت برای مدت فوق توصیه می‌شود."
                    : "This is to certify that:\n\nPatient: " + m.Patient + "\nIssue date: " + m.Date + "\nRecommended leave: " + m.Days + " day(s)\nClinical reason: " + (string.IsNullOrEmpty(m.Reason) ? "Based on clinical assessment" : m.Reason) + "\n\nMedical leave is recommended for the stated period.";
                g.DrawString(content, font, ink, rect, fmt);
            }
            using (var pen = new Pen(teal, 2)) g.DrawLine(pen, x + 55, y + 635, x + w - 55, y + 635);
            using (var ink = new SolidBrush(navy))
            using (var bold = new Font(rtl ? "Tahoma" : "Segoe UI", 12, FontStyle.Bold))
            using (var small = new Font(rtl ? "Tahoma" : "Segoe UI", 9))
            {
                g.DrawString(rtl ? ClinicBrand.DoctorNameFa : ClinicBrand.DoctorNameEn, bold, ink, x + 60, y + 658);
                g.DrawString(rtl ? "امضا و مهر پزشک" : "Doctor signature and stamp", small, ink, x + 60, y + 697);
                if (!string.IsNullOrWhiteSpace(m.Phone)) g.DrawString(m.Phone, small, ink, x + 60, y + 785);
                g.DrawString("Ashkan Matab", small, ink, x + 60, y + 810);
            }
        }

        // GDI+ has no built-in rounded rectangle primitive in .NET Framework 4.8.
        private static void DrawRoundedRectangleSafe(this Graphics g, Pen pen, Rectangle r, int radius)
        {
            using (var path = new GraphicsPath())
            {
                int d = radius * 2;
                path.AddArc(r.Left, r.Top, d, d, 180, 90);
                path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
                path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }
        }
    }
}
