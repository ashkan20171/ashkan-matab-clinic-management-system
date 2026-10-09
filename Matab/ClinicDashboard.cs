using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Matab
{
    // Read-only dashboard. No schema migrations or changes to patient records.
    internal sealed class ClinicDashboard : Form
    {
        private readonly FlowLayoutPanel cards;
        private readonly Label heading;
        private readonly Label subtitle;
        private readonly Button refresh;
        private readonly DataGridView appointments;
        private readonly Label message;
        private readonly Panel sidebar;
        private readonly FlowLayoutPanel navigation;
        private readonly Label brand;
        private readonly Button patientsLink;
        private readonly Button schedulerLink;
        private readonly Button recordsLink;
        private readonly Button prescriptionsLink;

        public ClinicDashboard()
        {
            Text = ClinicBrand.DashboardTitle;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(820, 560);
            Size = new Size(1160, 740);
            BackColor = Color.FromArgb(241, 247, 251);
            Font = new Font("Segoe UI", 10F);
            var header = new Panel { Dock = DockStyle.Top, Height = 148, BackColor = Color.FromArgb(12, 42, 65) };
            heading = new Label { AutoSize = false, Dock = DockStyle.Top, Height = 60, Font = new Font("Segoe UI", 21F, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter };
            subtitle = new Label { AutoSize = false, Dock = DockStyle.Top, Height = 30, ForeColor = Color.FromArgb(187, 220, 231), TextAlign = ContentAlignment.MiddleCenter };
            header.Controls.Add(subtitle);
            header.Controls.Add(heading);
            Controls.Add(header);
            sidebar = new Panel { Dock = DockStyle.Right, Width = 252, BackColor = Color.FromArgb(12, 42, 65), Padding = new Padding(16) };
            navigation = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
            brand = new Label { Dock = DockStyle.Top, Height = 104, ForeColor = Color.FromArgb(121, 225, 229), Font = new Font("Segoe UI", 16F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Text = "MATAB" };
            patientsLink = NavigationButton();
            schedulerLink = NavigationButton();
            recordsLink = NavigationButton();
            prescriptionsLink = NavigationButton();
            navigation.Controls.Add(patientsLink);
            navigation.Controls.Add(schedulerLink);
            navigation.Controls.Add(recordsLink);
            navigation.Controls.Add(prescriptionsLink);
            sidebar.Controls.Add(navigation);
            sidebar.Controls.Add(brand);
            Controls.Add(sidebar);
            patientsLink.Click += (s, e) => { using (var f = new frmParvandeh()) f.ShowDialog(this); };
            schedulerLink.Click += (s, e) => { using (var f = new AppointmentScheduler()) f.ShowDialog(this); };
            recordsLink.Click += (s, e) => { using (var f = new ElectronicMedicalRecordForm()) f.ShowDialog(this); };
            prescriptionsLink.Click += (s, e) => { using (var f = new PrescriptionManager()) f.ShowDialog(this); };
            cards = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 188, Padding = new Padding(18), AutoScroll = true, WrapContents = false };
            Controls.Add(cards);
            refresh = new Button { Dock = DockStyle.Top, Height = 48, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 130, 146), ForeColor = Color.White };
            refresh.FlatAppearance.BorderSize = 0;
            refresh.Click += (s, e) => Reload();
            Controls.Add(refresh);
            message = new Label { Dock = DockStyle.Bottom, Height = 34, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(136, 49, 49) };
            Controls.Add(message);
            appointments = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40, ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            Controls.Add(appointments);
            appointments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(225, 241, 245);
            appointments.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(16, 48, 69);
            appointments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 251);
            appointments.DefaultCellStyle.SelectionBackColor = Color.FromArgb(20, 123, 140);
            appointments.RowTemplate.Height = 40;
            appointments.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            appointments.GridColor = Color.FromArgb(228, 238, 244);
            appointments.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);
            appointments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            appointments.BringToFront();
            header.BringToFront();
            ApplyLanguage();
            Reload();
            Activated += (s, e) => ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            bool en = ClinicLanguage.IsEnglish;
            // Avoid RightToLeftLayout mutation while visible: WinForms may
            // recreate the native handle and close the application's main form.
            if (!IsHandleCreated) {
                RightToLeft = en ? RightToLeft.No : RightToLeft.Yes;
                RightToLeftLayout = !en;
            }
            sidebar.Dock = en ? DockStyle.Left : DockStyle.Right;
            navigation.RightToLeft = en ? RightToLeft.No : RightToLeft.Yes;
            cards.FlowDirection = en ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
            brand.Text = en ? "ASHKAN MATAB" : "اشکان مطب";
            patientsLink.Text = en ? "Patient records" : "پرونده بیماران";
            schedulerLink.Text = en ? "Appointments" : "نوبت‌دهی ساعتی";
            recordsLink.Text = en ? "Medical records" : "سوابق پزشکی";
            prescriptionsLink.Text = en ? "Prescriptions" : "نسخه‌ها و داروها";
            appointments.RightToLeft = en ? RightToLeft.No : RightToLeft.Yes;
            Text = ClinicBrand.DashboardTitle;
            heading.Text = en ? "Clinic overview" : "نمای کلی مطب";
            subtitle.Text = ClinicBrand.DoctorName;
            // Doctor name is displayed in the dashboard header in the selected language.
            refresh.Text = en ? "Refresh dashboard" : "به‌روزرسانی داشبورد";
        }

        internal void RefreshLanguageWithoutRecreatingHandle()
        {
            if (IsDisposed || Disposing) return;
            ApplyLanguage();
            // Language changes must not cause another database query or reset the grid.
            // Refresh the labels of the existing cards, retaining their numbers.
            string[] labels = ClinicLanguage.IsEnglish
                ? new[] { "Patients", "Appointments", "Visits", "Services" }
                : new[] { "پرونده‌های بیماران", "کل نوبت‌ها", "ویزیت‌ها", "خدمات" };
            int index = 0;
            foreach (Control card in cards.Controls)
            {
                if (index >= labels.Length) break;
                foreach (Control child in card.Controls)
                    if (child is Label && child.Dock == DockStyle.Top) child.Text = labels[index];
                index++;
            }
            string[] names = { "Nobat", "Tarikh", "NameBemar", "NameKh", "Tel" };
            string[] fa = { "شماره نوبت", "تاریخ شمسی", "نام", "نام خانوادگی", "تلفن" };
            string[] en = { "Number", "Date (Jalali)", "First name", "Last name", "Phone" };
            for (int i = 0; i < names.Length; i++)
                if (appointments.Columns.Contains(names[i]))
                    appointments.Columns[names[i]].HeaderText = ClinicLanguage.IsEnglish ? en[i] : fa[i];
        }

        private static Button NavigationButton()
        {
            var button = new Button { Width = 210, Height = 54, Margin = new Padding(3, 7, 3, 7), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(23, 74, 98), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(0, 135, 151);
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(23, 74, 98);
            return button;
        }

        private void AddCard(string title, int count)
        {
            var panel = new ClinicMetricCard { Width = 216, Height = 128, Margin = new Padding(10), BackColor = Color.White };
            var label = new Label { Dock = DockStyle.Top, Height = 46, Text = title, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(63, 89, 105) };
            var number = new Label { Dock = DockStyle.Fill, Text = count.ToString("N0"), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 25F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 119, 137) };
            panel.Controls.Add(number);
            panel.Controls.Add(label);
            cards.Controls.Add(panel);
        }

        private void Reload()
        {
            ApplyLanguage();
            cards.Controls.Clear();
            message.Text = "";
            bool en = ClinicLanguage.IsEnglish;
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                {
                    connection.Open();
                    AddCard(en ? "Patients" : "پرونده‌های بیماران", Count(connection, "SELECT COUNT(*) FROM dbo.Parvandeh"));
                    AddCard(en ? "Appointments" : "کل نوبت‌ها", Count(connection, "SELECT COUNT(*) FROM dbo.Nobat"));
                    AddCard(en ? "Visits" : "ویزیت‌ها", Count(connection, "SELECT COUNT(*) FROM dbo.Vizit"));
                    AddCard(en ? "Services" : "خدمات", Count(connection, "SELECT COUNT(*) FROM dbo.Khadamat"));
                    using (var adapter = new SqlDataAdapter("SELECT TOP (100) Nobat, Tarikh, NameBemar, NameKh, Tel FROM dbo.Nobat ORDER BY Nobat DESC", connection))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        appointments.DataSource = table;
                        if (appointments.Columns.Contains("Nobat")) appointments.Columns["Nobat"].HeaderText = en ? "Number" : "شماره نوبت";
                        if (appointments.Columns.Contains("Tarikh")) appointments.Columns["Tarikh"].HeaderText = en ? "Date (Jalali)" : "تاریخ شمسی";
                        if (appointments.Columns.Contains("NameBemar")) appointments.Columns["NameBemar"].HeaderText = en ? "First name" : "نام";
                        if (appointments.Columns.Contains("NameKh")) appointments.Columns["NameKh"].HeaderText = en ? "Last name" : "نام خانوادگی";
                        if (appointments.Columns.Contains("Tel")) appointments.Columns["Tel"].HeaderText = en ? "Phone" : "تلفن";
                    }
                }
            }
            catch (SqlException)
            {
                message.Text = en ? "Database connection or schema error. Check SQL Server and Matab database." : "خطا در اتصال یا ساختار دیتابیس؛ SQL Server و دیتابیس Matab را بررسی کنید.";
            }
            catch (InvalidOperationException)
            {
                message.Text = en ? "Unable to load dashboard data." : "بارگذاری اطلاعات داشبورد امکان‌پذیر نیست.";
            }
        }

        private static int Count(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection))
                return Convert.ToInt32(command.ExecuteScalar());
        }
    }
}
