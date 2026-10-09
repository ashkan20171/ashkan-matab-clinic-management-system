using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Matab
{
    // Administrative prescription log; does not suggest medication or dosage.
    internal sealed class PrescriptionManager : Form
    {
        readonly ComboBox patient = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 340, DisplayMember = "Display", ValueMember = "CodeP" };
        readonly TextBox filter = new TextBox { Width = 190 };
        readonly TextBox medicine = new TextBox { MaxLength = 250 };
        readonly TextBox instructions = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical, MaxLength = 2000 };
        readonly TextBox prescriber = new TextBox { MaxLength = 200 };
        readonly DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false };
        readonly Label status = new Label { Dock = DockStyle.Bottom, Height = 36, TextAlign = ContentAlignment.MiddleCenter };
        bool English { get { return ClinicLanguage.IsEnglish; } }
        string T(string fa, string en) { return English ? en : fa; }
        public PrescriptionManager()
        {
            Text = T("ثبت نسخه و داروهای بیمار", "Patient prescriptions");
            Size = new Size(1050, 720); MinimumSize = new Size(820, 550);
            StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(244, 249, 252);
            RightToLeft = English ? RightToLeft.No : RightToLeft.Yes;
            RightToLeftLayout = !English;
            var searchBar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10), AutoScroll = true };
            searchBar.Controls.Add(new Label { Text = T("جست‌وجوی بیمار", "Find patient"), AutoSize = true, Margin = new Padding(5, 10, 5, 0) });
            searchBar.Controls.Add(filter); searchBar.Controls.Add(patient);
            var searchButton = MakeButton(T("جست‌وجو", "Search"));
            searchButton.Click += (s, e) => LoadPatients(); searchBar.Controls.Add(searchButton);
            var fields = new TableLayoutPanel { Dock = DockStyle.Top, Height = 205, Padding = new Padding(12), ColumnCount = 2, RowCount = 3 };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 73));
            AddField(fields, 0, T("نام دارو", "Medication name"), medicine);
            AddField(fields, 1, T("دستور ثبت‌شده پزشک", "Prescriber's instructions"), instructions);
            AddField(fields, 2, T("نام پزشک", "Prescriber"), prescriber);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54, Padding = new Padding(9) };
            var save = MakeButton(T("ثبت نسخه", "Save prescription")); save.Click += (s, e) => Save();
            var refresh = MakeButton(T("بازخوانی", "Refresh")); refresh.Click += (s, e) => LoadRecords();
            actions.Controls.Add(save); actions.Controls.Add(refresh);
            Controls.Add(grid); Controls.Add(status); Controls.Add(actions); Controls.Add(fields); Controls.Add(searchBar);
            patient.SelectedIndexChanged += (s, e) => LoadRecords();
            Load += (s, e) => LoadPatients();
        }
        Button MakeButton(string title) { return new Button { Text = title, AutoSize = true, MinimumSize = new Size(110, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(12, 119, 135), ForeColor = Color.White }; }
        void AddField(TableLayoutPanel table, int row, string title, Control control)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 3F));
            table.Controls.Add(new Label { Text = title, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            control.Dock = DockStyle.Fill; table.Controls.Add(control, 1, row);
        }
        int? PatientId
        {
            get { var item = patient.SelectedItem as DataRowView; return item == null ? (int?)null : Convert.ToInt32(item["CodeP"]); }
        }
        void LoadPatients()
        {
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var cmd = new SqlCommand("SELECT CodeP, CAST(CodeP AS NVARCHAR(20)) + N' - ' + ISNULL(NameBemar,N'') + N' ' + ISNULL(NameKh,N'') AS Display FROM dbo.Parvandeh WHERE (@q=N'' OR NameBemar LIKE @pattern OR NameKh LIKE @pattern OR CAST(CodeP AS NVARCHAR(20))=@q) ORDER BY CodeP DESC", con))
                {
                    var q = filter.Text.Trim();
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 100).Value = q;
                    cmd.Parameters.Add("@pattern", SqlDbType.NVarChar, 210).Value = "%" + q.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";
                    var dt = new DataTable(); using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(dt);
                    patient.DataSource = dt; status.Text = "";
                }
            }
            catch (SqlException) { status.Text = T("اتصال به دیتابیس یا جدول بیماران برقرار نشد.", "Database connection or patient table unavailable."); }
        }
        void LoadRecords()
        {
            if (!PatientId.HasValue) { grid.DataSource = null; return; }
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var cmd = new SqlCommand("SELECT PrescriptionId, PrescribedAtUtc, MedicationName, Directions, Prescriber FROM dbo.PatientPrescriptions WHERE PatientCode=@id ORDER BY PrescriptionId DESC", con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = PatientId.Value;
                    var dt = new DataTable(); using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(dt);
                    grid.DataSource = dt; status.Text = "";
                    string[] fa = { "شناسه", "تاریخ ثبت (UTC)", "نام دارو", "دستور پزشک", "پزشک" };
                    string[] en = { "ID", "Recorded (UTC)", "Medication", "Instructions", "Prescriber" };
                    for (int i = 0; i < grid.Columns.Count && i < 5; i++) grid.Columns[i].HeaderText = English ? en[i] : fa[i];
                }
            }
            catch (SqlException) { status.Text = T("ابتدا اسکریپت Stage13_Prescriptions.sql را اجرا کنید.", "Run Stage13_Prescriptions.sql first."); }
        }
        void Save()
        {
            if (!PatientId.HasValue || string.IsNullOrWhiteSpace(medicine.Text) || string.IsNullOrWhiteSpace(prescriber.Text))
            { status.Text = T("بیمار، نام دارو و پزشک الزامی هستند.", "Patient, medication and prescriber are required."); return; }
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var cmd = new SqlCommand("INSERT INTO dbo.PatientPrescriptions(PatientCode,MedicationName,Directions,Prescriber) VALUES (@patient,@medicine,@directions,@prescriber)", con))
                {
                    cmd.Parameters.Add("@patient", SqlDbType.Int).Value = PatientId.Value;
                    cmd.Parameters.Add("@medicine", SqlDbType.NVarChar, 250).Value = medicine.Text.Trim();
                    cmd.Parameters.Add("@directions", SqlDbType.NVarChar, 2000).Value = string.IsNullOrWhiteSpace(instructions.Text) ? (object)DBNull.Value : instructions.Text.Trim();
                    cmd.Parameters.Add("@prescriber", SqlDbType.NVarChar, 200).Value = prescriber.Text.Trim();
                    con.Open(); cmd.ExecuteNonQuery();
                }
                medicine.Clear(); instructions.Clear(); LoadRecords();
                status.Text = T("نسخه ثبت شد.", "Prescription saved.");
            }
            catch (SqlException) { status.Text = T("ثبت ناموفق بود؛ دیتابیس و دسترسی را بررسی کنید.", "Save failed; check database and permissions."); }
        }
    }
}
