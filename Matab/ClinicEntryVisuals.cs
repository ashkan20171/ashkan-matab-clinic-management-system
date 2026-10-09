using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Matab
{
    // Non-destructive visual styling for legacy DotNetBar patient/appointment forms.
    // No control is reparented or removed; Designer remains the layout authority.
    internal static class ClinicEntryVisuals
    {
        private static readonly Color Navy = Color.FromArgb(23, 49, 71);
        private static readonly Color Teal = Color.FromArgb(0, 137, 153);
        private static readonly Color Canvas = Color.FromArgb(239, 246, 249);
        private static readonly Color Muted = Color.FromArgb(84, 108, 124);

        public static void Attach(Form form, string persianTitle, string englishTitle)
        {
            form.Load += (sender, args) => Apply(form, persianTitle, englishTitle);
            // Activated also refreshes styling after language changes without recreating handles.
            form.Activated += (sender, args) => Apply(form, persianTitle, englishTitle);
        }

        private static void Apply(Form form, string fa, string en)
        {
            if (form.IsDisposed) return;
            bool english = ClinicLanguage.IsEnglish;
            form.Text = (english ? "Ashkan Matab | " + en : "اشکان مطب | " + fa);
            form.BackColor = Canvas;
            form.RightToLeft = english ? RightToLeft.No : RightToLeft.Yes;
            foreach (Control control in form.Controls.Cast<Control>().ToArray()) Style(control, english);
        }

        private static void Style(Control control, bool english)
        {
            if (control.IsDisposed) return;
            string type = control.GetType().Name;
            if (control is Label)
            {
                control.ForeColor = Navy;
                control.Font = new Font("Tahoma", 9.5f, FontStyle.Bold);
            }
            else if (type == "GroupPanel" || control is GroupBox)
            {
                control.BackColor = Color.White;
                control.ForeColor = Navy;
            }
            else if (type == "ButtonX" || control is Button)
            {
                bool destructive = control.Name.IndexOf("delete", StringComparison.OrdinalIgnoreCase) >= 0;
                control.BackColor = destructive ? Color.FromArgb(179, 65, 77) : Teal;
                control.ForeColor = Color.White;
                control.Font = new Font("Tahoma", 9f, FontStyle.Bold);
                if (control is Button standard)
                {
                    standard.FlatStyle = FlatStyle.Flat;
                    standard.FlatAppearance.BorderSize = 0;
                }
            }
            else if (control is TextBoxBase || type == "TextBoxX" || type == "IntegerInput" || type == "MaskedTextBoxAdv")
            {
                control.BackColor = Color.White;
                control.ForeColor = Navy;
                control.Font = new Font("Tahoma", 9.5f);
            }
            else if (control is ComboBox)
            {
                control.BackColor = Color.White;
                control.ForeColor = Muted;
            }
            // Numeric identifiers and date masks retain their own text direction.
            foreach (Control child in control.Controls.Cast<Control>().ToArray()) Style(child, english);
        }
    }
}
