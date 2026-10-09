using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Matab
{
    // UI-only localization: database values and business rules remain unchanged.
    internal static class ClinicLanguage
    {
        private static bool english;
        private static bool switching;
        // Controls keep their original designer captions so language switching is reversible.
        private static readonly Dictionary<Form, string> FormOriginals = new Dictionary<Form, string>();
        private static readonly Dictionary<Control, string> Originals = new Dictionary<Control, string>();
        private static readonly Dictionary<ToolStripItem, string> MenuOriginals = new Dictionary<ToolStripItem, string>();
        private static readonly Dictionary<DataGridViewColumn, string> ColumnOriginals = new Dictionary<DataGridViewColumn, string>();
        private static readonly Dictionary<string, string> En = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            {"دکتر اشکان مطاعی", "dr.ashkan motaei"}, {"اشکان مطب", "Ashkan Matab"}, {"اشکان مطب | مدیریت مطب", "Ashkan Matab | Clinic Management"},
            {"به سامانه مدیریت مطب خوش آمدید", "Welcome to Matab"}, {"ورود امن به سامانه مطب", "Secure clinic access"}, {"فرم ورود به سیستم نوبت دهی", "Clinic sign in"}, {"سطح دسترسی", "Access level"}, {"کلمه عبور", "Password"}, {"ورود", "Sign in"}, {"نام کاربری", "Username"}, {"رمز عبور", "Password"},
            {"مدیر", "Administrator"}, {"کاربر عادی", "Standard user"},
            {"تنظیمات", "Settings"}, {"کاربران", "Users"}, {"بیماران", "Patients"},
            {"پرونده", "Patient record"}, {"پرونده بیماران", "Patient records"},
            {"نوبت", "Appointment"}, {"نوبت دهی", "Appointments"}, {"نوبت دهی بیماران", "Patient appointments"},
            {"لیست نوبت ها", "Appointment list"}, {"ویزیت", "Visit"}, {"لیست ویزیت", "Visit list"},
            {"بیمه", "Insurance"}, {"خدمات", "Services"}, {"گواهی", "Certificate"},
            {"هزینه", "Expense"}, {"هزینه ها", "Expenses"}, {"پرداخت", "Payment"},
            {"پرداخت ها", "Payments"}, {"منشی", "Receptionist"}, {"لیست منشی", "Receptionists"},
            {"ثبت", "Save"}, {"ذخیره", "Save"}, {"حذف", "Delete"}, {"ویرایش", "Edit"},
            {"جدید", "New"}, {"افزودن", "Add"}, {"جستجو", "Search"}, {"بازگشت", "Back"},
            {"خروج", "Exit"}, {"انصراف", "Cancel"}, {"چاپ", "Print"}, {"گزارش", "Report"},
            {"تاریخ", "Date"}, {"نام", "First name"}, {"نام خانوادگی", "Last name"},
            {"تلفن", "Phone"}, {"تلفن تماس", "Phone number"}, {"آدرس", "Address"},
            {"توضیحات", "Notes"}, {"مبلغ", "Amount"}, {"کد", "Code"}, {"ردیف", "No."},
            {"شنبه", "Saturday"}, {"یکشنبه", "Sunday"}, {"دوشنبه", "Monday"},
            {"سه شنبه", "Tuesday"}, {"چهارشنبه", "Wednesday"}, {"چهرشنبه", "Wednesday"},
            {"پنج شنبه", "Thursday"}, {"جمعه", "Friday"},
            {"بستن", "Close"}, {"تایید", "Confirm"}, {"تأیید", "Confirm"},
            {"پاک کردن", "Clear"}, {"نمایش", "View"}, {"لیست بیماران", "Patients list"},
            {"شماره پرونده", "Record number"}, {"شماره تماس", "Contact number"},
            {"کد ملی", "National ID"}, {"پزشک", "Doctor"}, {"ساعت", "Time"},
            {"وضعیت", "Status"}, {"مراجعه", "Visit"}, {"بیمار", "Patient"},
            {"تعداد", "Count"}, {"جمع", "Total"}, {"جمع کل", "Grand total"},
            {"روز", "Day"}, {"ماه", "Month"}, {"سال", "Year"},
            {"قبلی", "Previous"}, {"بعدی", "Next"}, {"انتخاب", "Select"},
            {"گواهی استراحت پزشکی", "Medical leave certificate"},
            {"گواهی استراحت", "Medical leave certificate"},
            {"گزارش عملکرد", "Performance report"},
            {"گزارشات", "Reports"},
            {"مشخصات بیمار", "Patient details"},
            {"مشخصات مطب", "Clinic details"},
            {"ثبت بیمار", "Register patient"},
            {"ثبت نوبت", "Book appointment"},
            {"لیست پرونده ها", "Patient records"},
            {"شماره نوبت", "Appointment number"},
            {"نام پدر", "Father’s name"},
            {"تاریخ تولد", "Date of birth"},
            {"جنسیت", "Gender"},
            {"مرد", "Male"},
            {"زن", "Female"},
            {"بیماری فعلی", "Current condition"},
            {"بیماری قبلی", "Medical history"},
            {"نوع بیمه", "Insurance type"},
            {"مبلغ بیمه", "Insurance amount"},
            {"مبلغ خدمات", "Service amount"},
            {"جمع پرداختی", "Total paid"},
            {"از تاریخ", "From date"},
            {"تا تاریخ", "To date"},
            {"تاریخ شروع", "Start date"},
            {"تاریخ پایان", "End date"},
            {"نام مطب", "Clinic name"},
            {"شماره تلفن", "Telephone"},
            {"نام پزشک", "Doctor name"},
            {"تاریخ مراجعه", "Visit date"},
            {"مدت استراحت", "Leave duration"},
            {"روز استراحت", "Leave days"},
            {"علت مراجعه", "Reason for visit"},
            {"دارو", "Medication"},
            {"نسخه", "Prescription"},
            {"ثبت اطلاعات", "Save information"},
            {"ویرایش اطلاعات", "Edit information"},
            {"حذف اطلاعات", "Delete information"},
            {"چاپ گواهی", "Print certificate"},
            {"چاپ گزارش", "Print report"},
            {"خانه", "Home"},
            {"داشبورد", "Dashboard"},
            {"تنظیمات مطب", "Clinic settings"},
            {"درباره ما", "About"},
            {"خروج از سیستم", "Sign out"},
            {"ورود به سیستم", "Sign in"},
            {"اطلاعات بیمار", "Patient information"},
            {"تعداد ویزیت", "Visit count"},
            {"کل درآمد", "Total revenue"},
            {"بیمه پرداختی", "Insurance payments"},
            {"خدمات ارائه شده", "Services provided"},
            {"تاریخ امروز", "Today"},
            {"آدرس مطب", "Clinic address"},
            {"توضیحات تکمیلی", "Additional notes"},
            {"شماره ملی", "National ID"},
            {"شماره همراه", "Mobile number"},
            {"موبایل", "Mobile"},
            {"کد بیمار", "Patient code"},
            {"پزشکان", "Doctors"},
            {"نوبت های امروز", "Today’s appointments"},
            {"لیست خدمات", "Service list"},
            {"مبلغ کل", "Total amount"},
            {"تاریخ ثبت", "Registration date"},
            {"ساعت مراجعه", "Appointment time"},
            {"تاریخ نوبت", "Appointment date"},
            {"تعداد روز", "Number of days"},
            {"مشاهده", "View"},
            {"ذخیره تغییرات", "Save changes"},
            {"بروزرسانی", "Update"},
            {"به روز رسانی", "Update"},
            {"نام بیمار", "Patient name"},
            {"جستجوی بیمار", "Search patients"},
            {"جستجوی نوبت", "Search appointments"},
            {"ثبت ویزیت", "Record visit"},
            {"لیست هزینه ها", "Expense list"},
            {"درآمد", "Revenue"},
            {"گزارش مالی", "Financial report"},
            {"گواهی پزشکی", "Medical certificate"},
            {"شماره", "Number"}, {"مشخصات", "Details"}, {"لیست", "List"},
            {"نام و نام خانوادگی", "Full name"}, {"تاریخ صدور", "Issue date"},
            {"مدت", "Duration"}, {"روزهای استراحت", "Leave days"},
            {"مراجعه کننده", "Patient"}, {"نوع خدمات", "Service type"},
            {"جمع مبلغ", "Total amount"}, {"مبلغ پرداختی", "Paid amount"},
            {"تخفیف", "Discount"}, {"مانده", "Balance"},
            {"پرینت", "Print"}, {"گزارش گیری", "Generate report"},
            {"نام پزشک معالج", "Attending physician"},
            {"تاریخ ویزیت", "Visit date"}, {"علت بیماری", "Diagnosis"},
            {"تاریخ مراجعه بیمار", "Patient visit date"},
            {"اطلاعات مطب", "Clinic information"},
            {"بازیابی", "Restore"}, {"پشتیبان گیری", "Backup"},
            {"ثبت جدید", "Add new"}, {"ویرایش بیمار", "Edit patient"},
            {"جست و جو", "Search"}, {"جست و جو...", "Search..."},
            {"تاریخ شمسی", "Persian date"}, {"ساعت نوبت", "Appointment time"}
        };
        static ClinicLanguage()
        {
            try { english = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Matab", "language.en")); }
            catch { english = false; }
        }
        public static bool IsEnglish { get { return english; } }
        public static void Attach(Form form)
        {
            if (form == null || form.IsDisposed) return;
            if (!FormOriginals.ContainsKey(form)) FormOriginals[form] = form.Text ?? "";
            form.Text = english ? Translate(FormOriginals[form]) : FormOriginals[form];
            if (!form.IsHandleCreated)
                form.RightToLeft = english ? RightToLeft.No : RightToLeft.Yes;
            // Never mutate RightToLeftLayout after a WinForms handle exists.
            if (!form.IsHandleCreated) form.RightToLeftLayout = !english;
            ApplyDirection(form, !form.IsHandleCreated);
            ApplyTree(form);
            if (FindSwitcher(form) != null) return;
            var switcher = new Button { Name = "matabLanguageSwitch", Text = english ? "فارسی | FA" : "English | EN", AutoSize = false, Width = 112, Height = 31, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(20, 105, 132), ForeColor = Color.White, TabStop = false };
            switcher.FlatAppearance.BorderSize = 0;
            switcher.Click += (s, e) => Toggle();
            form.Controls.Add(switcher);
            switcher.BringToFront();
            form.FormClosed += (s, e) => { /* no global form retention */ };
        }
        private static void Toggle()
        {
            if (switching) return;
            switching = true;
            bool previous = english;
            try
            {
                english = !english;
                // Snapshot before changing any controls: events may alter OpenForms.
                Form[] forms = Application.OpenForms.Cast<Form>().ToArray();
                foreach (Form f in forms)
                {
                    if (f == null || f.IsDisposed || f.Disposing) continue;
                    try
                    {
                        f.SuspendLayout();
                        // Never touch RightToLeftLayout or recreate a top-level handle
                        // while the application's login-owned message loop is active.
                        if (!FormOriginals.ContainsKey(f)) FormOriginals[f] = f.Text ?? "";
                        f.Text = english ? Translate(FormOriginals[f]) : FormOriginals[f];
                        ApplyDirection(f, false);
                        ApplyTree(f);
                        var switcher = FindSwitcher(f);
                        if (switcher != null)
                            switcher.Text = english ? "فارسی | FA" : "English | EN";
                        var dashboard = f as ClinicDashboard;
                        if (dashboard != null)
                            dashboard.RefreshLanguageWithoutRecreatingHandle();
                    }
                    catch (ObjectDisposedException) { }
                    catch (InvalidOperationException) { }
                    finally
                    {
                        if (!f.IsDisposed && !f.Disposing) f.ResumeLayout(true);
                    }
                }
                SavePreference();
            }
            catch (Exception ex)
            {
                english = previous;
                MessageBox.Show(
                    (english ? "Language change failed: " : "خطا در تغییر زبان: ") + ex.Message,
                    "Ashkan Matab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { switching = false; }
        }
        private static void SavePreference()
        {
            try
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Matab");
                Directory.CreateDirectory(folder);
                var marker = Path.Combine(folder, "language.en");
                if (english) File.WriteAllText(marker, "en", Encoding.UTF8);
                else if (File.Exists(marker)) File.Delete(marker);
            }
            catch (IOException) { /* continue with in-memory preference */ }
            catch (UnauthorizedAccessException) { }
        }
        private static Control FindSwitcher(Control parent)
        {
            foreach (Control c in parent.Controls.Cast<Control>().ToArray())
                if (c.Name == "matabLanguageSwitch") return c;
            return null;
        }
        private static void ApplyTree(Control parent)
        {
            foreach (Control c in parent.Controls.Cast<Control>().ToArray())
            {
                if (c.IsDisposed || c.Name == "matabLanguageSwitch") continue;
                if (!Originals.ContainsKey(c)) Originals[c] = c.Text ?? "";
                string source = Originals[c].Trim();
                string translation;
                if (c is Label || c is Button || c is GroupBox || c is CheckBox || c is RadioButton || c is LinkLabel || c is TabPage)
                {
                    c.Text = english ? Translate(source) : Originals[c];
                }
                // Never translate ComboBox entries or textbox values: legacy SQL depends on them.
                if (c is DataGridView)
                {
                    var grid = (DataGridView)c;
                    grid.RightToLeft = english ? RightToLeft.No : RightToLeft.Yes;
                    foreach (DataGridViewColumn column in grid.Columns.Cast<DataGridViewColumn>().ToArray())
                    {
                        if (!ColumnOriginals.ContainsKey(column)) ColumnOriginals[column] = column.HeaderText ?? "";
                        column.HeaderText = english ? Translate(ColumnOriginals[column]) : ColumnOriginals[column];
                    }
                }
                if (c is MenuStrip || c is ContextMenuStrip || c is ToolStrip)
                {
                    var strip = (ToolStrip)c;
                    foreach (ToolStripItem item in strip.Items.Cast<ToolStripItem>().ToArray()) ApplyMenu(item);
                }
                if (c is TabControl)
                {
                    foreach (TabPage page in ((TabControl)c).TabPages.Cast<TabPage>().ToArray())
                    {
                        if (!Originals.ContainsKey(page)) Originals[page] = page.Text ?? "";
                        page.Text = english ? Translate(Originals[page]) : Originals[page];
                    }
                }
                if (c is Label && c.Name != "matabLanguageSwitch")
                {
                    var label = (Label)c;
                    if (label.TextAlign == ContentAlignment.MiddleLeft || label.TextAlign == ContentAlignment.MiddleRight)
                        label.TextAlign = english ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;
                }
                if (!c.IsDisposed) ApplyTree(c);
            }
        }
        // Only child controls are mirrored live. Recreating a visible top-level Form's
        // native handle can close the legacy Application.Run login window.
        private static void ApplyDirection(Form form, bool initial)
        {
            var direction = english ? RightToLeft.No : RightToLeft.Yes;
            if (initial) { form.RightToLeft = direction; form.RightToLeftLayout = !english; }
            // RightToLeft changes the direction of content without changing the
            // top-level RightToLeftLayout property (which can recreate the HWND).
            if (initial || !form.IsHandleCreated)
                form.RightToLeft = direction;
            foreach (Control child in form.Controls.Cast<Control>().ToArray())
                ApplyChildDirection(child, direction);
        }
        private static void ApplyChildDirection(Control child, RightToLeft direction)
        {
            if (child.IsDisposed) return;
            // Avoid changing text input values or recreating form handles.
            child.RightToLeft = direction;
            var flow = child as FlowLayoutPanel;
            if (flow != null && flow.FlowDirection != FlowDirection.TopDown && flow.FlowDirection != FlowDirection.BottomUp)
                flow.FlowDirection = direction == RightToLeft.Yes ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            var table = child as TableLayoutPanel;
            if (table != null) table.RightToLeft = direction;
            var strip = child as ToolStrip;
            if (strip != null) strip.RightToLeft = direction;
            var tabs = child as TabControl;
            if (tabs != null && !tabs.IsHandleCreated) tabs.RightToLeftLayout = direction == RightToLeft.Yes;
            foreach (Control nested in child.Controls.Cast<Control>().ToArray())
                ApplyChildDirection(nested, direction);
        }
        private static string Translate(string source)
        {
            string translated;
            return english && En.TryGetValue((source ?? "").Trim(), out translated) ? translated : source;
        }
        private static void ApplyMenu(ToolStripItem item)
        {
            if (!MenuOriginals.ContainsKey(item)) MenuOriginals[item] = item.Text ?? "";
            item.Text = english ? Translate(MenuOriginals[item]) : MenuOriginals[item];
            var dropdown = item as ToolStripDropDownItem;
            if (dropdown != null)
                foreach (ToolStripItem child in dropdown.DropDownItems.Cast<ToolStripItem>().ToArray()) ApplyMenu(child);
        }
    }
}
