using System;
using System.Drawing;
using System.Windows.Forms;

namespace Matab
{
    // Shared visual language for legacy patient and appointment lists.
    internal static class ClinicListVisuals
    {
        private static readonly Color Ink = Color.FromArgb(20, 49, 69);
        private static readonly Color Teal = Color.FromArgb(12, 128, 147);
        private static readonly Color Surface = Color.FromArgb(244, 249, 251);

        public static void Apply(Form form, DataGridView grid)
        {
            form.BackColor = Surface;
            form.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            // Avoid changing RightToLeftLayout on an already displayed WinForms window.
            form.RightToLeft = ClinicLanguage.IsEnglish ? RightToLeft.No : RightToLeft.Yes;
            StyleChildren(form);
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(229, 237, 242);
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 46;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Ink;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = ClinicLanguage.IsEnglish ? DataGridViewContentAlignment.MiddleLeft : DataGridViewContentAlignment.MiddleRight;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            grid.DefaultCellStyle.ForeColor = Ink;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = Teal;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(239, 247, 249);
            grid.RowTemplate.Height = 38;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
        }

        private static void StyleChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBoxBase)
                {
                    control.BackColor = Color.White;
                    control.ForeColor = Ink;
                }
                else if (control is Button)
                {
                    control.BackColor = Teal;
                    control.ForeColor = Color.White;
                }
                StyleChildren(control);
            }
        }
    }
}
