using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Matab
{
    // The legacy database stores dates as yyyyMMdd in the Persian calendar.
    // This selector uses a Gregorian DateTimePicker internally and converts only at the boundary.
    internal static class AppointmentDatePicker
    {
        internal static bool IsValidDate(string text)
        {
            DateTime parsed;
            return TryParseLegacy(text, out parsed);
        }

        internal static void Attach(Form form, Control field)
        {
            var button = new Button();
            button.Name = "btnAppointmentCalendar";
            button.Text = "📅";
            button.Font = new Font("Segoe UI", 10F);
            button.Size = new Size(42, Math.Max(27, field.Height));
            button.Location = new Point(Math.Max(0, field.Right + 5), field.Top);
            button.Anchor = field.Anchor;
            button.Click += (sender, args) => SelectDate(field);
            field.Parent.Controls.Add(button);
            button.BringToFront();
        }

        private static void SelectDate(Control field)
        {
            using (var dialog = new Form())
            using (var picker = new DateTimePicker())
            using (var confirm = new Button())
            {
                dialog.Text = ClinicLanguage.IsEnglish ? "Select appointment date" : "انتخاب تاریخ نوبت";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ClientSize = new Size(330, 125);
                dialog.RightToLeft = ClinicLanguage.IsEnglish ? RightToLeft.No : RightToLeft.Yes;
                picker.Format = DateTimePickerFormat.Custom;
                picker.CustomFormat = "yyyy/MM/dd";
                picker.Location = new Point(20, 20);
                picker.Width = 285;
                DateTime current;
                if (TryParseLegacy(field.Text, out current)) picker.Value = current;
                confirm.Text = ClinicLanguage.IsEnglish ? "Select" : "تأیید";
                confirm.Location = new Point(115, 70);
                confirm.Size = new Size(100, 32);
                confirm.DialogResult = DialogResult.OK;
                dialog.Controls.Add(picker);
                dialog.Controls.Add(confirm);
                dialog.AcceptButton = confirm;
                if (dialog.ShowDialog(field.FindForm()) == DialogResult.OK)
                {
                    var p = new PersianCalendar();
                    var date = picker.Value.Date;
                    field.Text = string.Format(CultureInfo.InvariantCulture, "{0:0000}{1:00}{2:00}",
                        p.GetYear(date), p.GetMonth(date), p.GetDayOfMonth(date));
                }
            }
        }

        internal static bool TryParseLegacy(string text, out DateTime result)
        {
            result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(text)) return false;
            var digits = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (c >= '0' && c <= '9') digits.Append(c);
                else if (c >= '۰' && c <= '۹') digits.Append((char)('0' + c - '۰'));
                else if (c >= '٠' && c <= '٩') digits.Append((char)('0' + c - '٠'));
            }
            if (digits.Length != 8) return false;
            try
            {
                var value = digits.ToString();
                int y = int.Parse(value.Substring(0, 4), CultureInfo.InvariantCulture);
                int m = int.Parse(value.Substring(4, 2), CultureInfo.InvariantCulture);
                int d = int.Parse(value.Substring(6, 2), CultureInfo.InvariantCulture);
                result = new PersianCalendar().ToDateTime(y, m, d, 0, 0, 0, 0);
                return true;
            }
            catch (ArgumentOutOfRangeException) { return false; }
            catch (ArgumentException) { return false; }
        }
    }
}
