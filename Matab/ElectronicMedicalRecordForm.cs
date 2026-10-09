using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Matab
{
    internal sealed class ElectronicMedicalRecordForm : Form
    {
        readonly TextBox search = new TextBox { Width = 210 };
        readonly ComboBox patients = new ComboBox { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Display", ValueMember = "CodeP" };
        readonly DateTimePicker encounter = new DateTimePicker { Width = 170, Format = DateTimePickerFormat.Short };
        readonly TextBox complaint = new TextBox();
        readonly TextBox history = new TextBox();
        readonly TextBox allergies = new TextBox();
        readonly TextBox medications = new TextBox();
        readonly TextBox assessment = new TextBox();
        readonly TextBox plan = new TextBox();
        readonly DataGridView records = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, RowHeadersVisible = false, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        readonly Label feedback = new Label { Dock = DockStyle.Bottom, Height = 36, TextAlign = ContentAlignment.MiddleCenter };
        readonly bool en = ClinicLanguage.IsEnglish;
        string T(string fa, string english) { return en ? english : fa; }

        public ElectronicMedicalRecordForm()
        {
            Text = T("پرونده الکترونیکی بیمار", "Electronic medical records");
            Size = new Size(1100, 760);
            MinimumSize = new Size(850, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(244, 249, 252);
            Font = new Font("Segoe UI", 10F);
            RightToLeft = en ? RightToLeft.No : RightToLeft.Yes;
            RightToLeftLayout = !en;
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 64, Padding = new Padding(12), AutoScroll = true };
            top.Controls.Add(new Label { Text = T("جست‌وجوی بیمار:", "Search patient:"), AutoSize = true, Margin = new Padding(6, 12, 6, 0) });
            top.Controls.Add(search);
            top.Controls.Add(patients);
            var reload = Button(T("جست‌وجو", "Search"));
            reload.Click += (s, e) => LoadPatients();
            top.Controls.Add(reload);
            var editor = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 2, RowCount = 7, Height = 352, Padding = new Padding(12), AutoScroll = true };
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
            AddField(editor, 0, T("تاریخ مراجعه", "Encounter date"), encounter);
            AddField(editor, 1, T("علت مراجعه", "Chief complaint"), complaint);
            AddField(editor, 2, T("سوابق پزشکی", "Medical history"), history);
            AddField(editor, 3, T("حساسیت‌ها", "Allergies"), allergies);
            AddField(editor, 4, T("داروهای مصرفی", "Medications"), medications);
            AddField(editor, 5, T("ارزیابی پزشک", "Assessment"), assessment);
            AddField(editor, 6, T("برنامه درمان", "Treatment plan"), plan);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(10) };
            var save = Button(T("ثبت یادداشت پزشکی", "Save clinical note"));
            save.Click += (s, e) => SaveNote();
            actions.Controls.Add(save);
            var refresh = Button(T("بازخوانی سوابق", "Refresh history"));
            refresh.Click += (s, e) => LoadNotes();
            actions.Controls.Add(refresh);
            Controls.Add(records);
            Controls.Add(feedback);
            Controls.Add(actions);
            Controls.Add(editor);
            Controls.Add(top);
            patients.SelectedIndexChanged += (s, e) => LoadNotes();
            Load += (s, e) => LoadPatients();
        }
        Button Button(string title)
        {
            return new Button { Text = title, AutoSize = true, MinimumSize = new Size(120, 32), BackColor = Color.FromArgb(12, 119, 135), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        }
        void AddField(TableLayoutPanel table, int row, string title, Control input)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            table.Controls.Add(new Label { Text = title, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            input.Dock = DockStyle.Fill;
            table.Controls.Add(input, 1, row);
        }
        int? PatientCode
        {
            get
            {
                var view = patients.SelectedItem as DataRowView;
                return view == null ? (int?)null : Convert.ToInt32(view["CodeP"]);
            }
        }
        void LoadPatients()
        {
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var cmd = new SqlCommand("SELECT CodeP, CAST(CodeP AS NVARCHAR(20)) + N' - ' + ISNULL(NameBemar,N'') + N' ' + ISNULL(NameKh,N'') AS Display FROM dbo.Parvandeh WHERE (@q=N'' OR NameBemar LIKE @pattern OR NameKh LIKE @pattern OR CAST(CodeP AS NVARCHAR(20))=@q) ORDER BY CodeP DESC", con))
                {
                    var q = search.Text.Trim();
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 100).Value = q;
                    cmd.Parameters.Add("@pattern", SqlDbType.NVarChar, 210).Value = "%" + q.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";
                    var data = new DataTable();
                    using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(data);
                    patients.DataSource = data;
                    feedback.Text = "";
                }
            }
            catch (SqlException) { feedback.Text = T("اتصال دیتابیس یا جدول بیماران را بررسی کنید.", "Check the database connection and patient table."); }
        }
        void LoadNotes()
        {
            if (!PatientCode.HasValue) { records.DataSource = null; return; }
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var cmd = new SqlCommand("SELECT NoteId, EncounterDate, ChiefComplaint, Allergies, Medications, Assessment, TreatmentPlan FROM dbo.ClinicalNotes WHERE PatientCode=@patient ORDER BY EncounterDate DESC, NoteId DESC", con))
                {
                    cmd.Parameters.Add("@patient", SqlDbType.Int).Value = PatientCode.Value;
                    var data = new DataTable();
                    using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(data);
                    records.DataSource = data;
                    feedback.Text = "";
                }
            }
            catch (SqlException) { feedback.Text = T("ابتدا اسکریپت Stage12_ElectronicMedicalRecords.sql را در SSMS اجرا کنید.", "Run Stage12_ElectronicMedicalRecords.sql in SSMS first."); }
        }
        void SaveNote()
        {
            if (!PatientCode.HasValue) { feedback.Text = T("بیمار را انتخاب کنید.", "Select a patient."); return; }
            if (string.IsNullOrWhiteSpace(complaint.Text) && string.IsNullOrWhiteSpace(assessment.Text))
            { feedback.Text = T("علت مراجعه یا ارزیابی را وارد کنید.", "Enter a complaint or assessment."); return; }
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var cmd = new SqlCommand("INSERT INTO dbo.ClinicalNotes(PatientCode,EncounterDate,ChiefComplaint,MedicalHistory,Allergies,Medications,Assessment,TreatmentPlan) VALUES (@patient,@date,@complaint,@history,@allergies,@medications,@assessment,@plan)", con))
                {
                    cmd.Parameters.Add("@patient", SqlDbType.Int).Value = PatientCode.Value;
                    cmd.Parameters.Add("@date", SqlDbType.DateTime2).Value = encounter.Value.Date;
                    AddText(cmd, "@complaint", complaint.Text);
                    AddText(cmd, "@history", history.Text);
                    AddText(cmd, "@allergies", allergies.Text);
                    AddText(cmd, "@medications", medications.Text);
                    AddText(cmd, "@assessment", assessment.Text);
                    AddText(cmd, "@plan", plan.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadNotes();
                feedback.Text = T("یادداشت ثبت شد.", "Clinical note saved.");
            }
            catch (SqlException) { feedback.Text = T("ثبت ناموفق بود؛ ساختار دیتابیس و اتصال را بررسی کنید.", "Save failed; check database schema and connection."); }
        }
        static void AddText(SqlCommand cmd, string name, string value)
        {
            cmd.Parameters.Add(name, SqlDbType.NVarChar, -1).Value = string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }
    }
}
