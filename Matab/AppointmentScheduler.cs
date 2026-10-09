using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Matab
{
    internal sealed class AppointmentScheduler : Form
    {
        readonly DateTimePicker date = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 140 };
        readonly DateTimePicker time = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 110 };
        readonly NumericUpDown duration = new NumericUpDown { Minimum = 5, Maximum = 240, Value = 30, Increment = 5, Width = 75 };
        readonly TextBox patient = new TextBox { Width = 180 };
        readonly TextBox phone = new TextBox { Width = 140 };
        readonly TextBox notes = new TextBox { Width = 220 };
        readonly ComboBox status = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 125 };
        readonly DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        readonly Label feedback = new Label { Dock = DockStyle.Bottom, Height = 34, ForeColor = Color.FromArgb(145, 50, 50), TextAlign = ContentAlignment.MiddleCenter };
        readonly FlowLayoutPanel fields = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 145, AutoScroll = true, Padding = new Padding(12), WrapContents = true };
        int? selectedId;
        bool En { get { return ClinicLanguage.IsEnglish; } }
        string T(string fa, string en) { return En ? en : fa; }
        public AppointmentScheduler()
        {
            Text = "Matab | Appointments";
            Size = new Size(1100, 680);
            MinimumSize = new Size(820, 550);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(244, 249, 252);
            Font = new Font("Segoe UI", 10);
            RightToLeft = En ? RightToLeft.No : RightToLeft.Yes;
            RightToLeftLayout = !En;
            status.Items.AddRange(new object[] { "Scheduled", "Arrived", "Completed", "Cancelled" });
            status.SelectedIndex = 0;
            AddField(T("تاریخ", "Date"), date);
            AddField(T("ساعت", "Time"), time);
            AddField(T("مدت (دقیقه)", "Minutes"), duration);
            AddField(T("نام بیمار", "Patient"), patient);
            AddField(T("تلفن", "Phone"), phone);
            AddField(T("وضعیت", "Status"), status);
            AddField(T("توضیحات", "Notes"), notes);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(10) };
            AddButton(actions, T("ثبت نوبت", "Book"), (s, e) => Save());
            AddButton(actions, T("تغییر وضعیت", "Update status"), (s, e) => UpdateStatus());
            AddButton(actions, T("لغو نوبت", "Cancel"), (s, e) => Cancel());
            AddButton(actions, T("بازخوانی", "Refresh"), (s, e) => LoadRows());
            var title = new Label { Dock = DockStyle.Top, Height = 66, BackColor = Color.FromArgb(17, 54, 75), ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Text = T("مدیریت نوبت‌های ساعتی", "Appointment scheduling") };
            Controls.Add(grid);
            Controls.Add(feedback);
            Controls.Add(actions);
            Controls.Add(fields);
            Controls.Add(title);
            grid.SelectionChanged += (s, e) => { if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].Cells["Id"].Value != null) selectedId = Convert.ToInt32(grid.SelectedRows[0].Cells["Id"].Value); };
            Load += (s, e) => LoadRows();
        }
        void AddField(string caption, Control input)
        {
            var p = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, Width = input.Width + 18, Height = 70, Margin = new Padding(5) };
            p.Controls.Add(new Label { Text = caption, AutoSize = true, ForeColor = Color.FromArgb(38, 76, 96) });
            p.Controls.Add(input);
            fields.Controls.Add(p);
        }
        void AddButton(FlowLayoutPanel parent, string title, EventHandler action)
        {
            var b = new Button { Text = title, AutoSize = true, Height = 36, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(12, 119, 135), ForeColor = Color.White, Margin = new Padding(5) };
            b.Click += action;
            parent.Controls.Add(b);
        }
        DateTime Start { get { return date.Value.Date.Add(time.Value.TimeOfDay); } }
        void Save()
        {
            if (string.IsNullOrWhiteSpace(patient.Text)) { feedback.Text = T("نام بیمار الزامی است.", "Patient name is required."); return; }
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                {
                    con.Open();
                    using (var tx = con.BeginTransaction(IsolationLevel.Serializable))
                    {
                        // Lock the appointment range so two concurrent bookings cannot overlap.
                        const string overlap = @"SELECT COUNT(1) FROM dbo.AppointmentSlots WITH (UPDLOCK, HOLDLOCK) WHERE Status<>N'Cancelled' AND AppointmentAt < DATEADD(minute,@duration,@start) AND DATEADD(minute,DurationMinutes,AppointmentAt)>@start";
                        using (var check = new SqlCommand(overlap, con, tx))
                        {
                            check.Parameters.Add("@start", SqlDbType.DateTime2).Value = Start;
                            check.Parameters.Add("@duration", SqlDbType.Int).Value = (int)duration.Value;
                            if (Convert.ToInt32(check.ExecuteScalar()) > 0) { tx.Rollback(); feedback.Text = T("این بازه زمانی قبلاً رزرو شده است.", "This time slot overlaps an existing booking."); return; }
                        }
                        using (var cmd = new SqlCommand("INSERT INTO dbo.AppointmentSlots(PatientName,Phone,AppointmentAt,DurationMinutes,Status,Notes) VALUES(@patient,@phone,@start,@duration,N'Scheduled',@notes)", con, tx))
                        {
                            cmd.Parameters.Add("@patient", SqlDbType.NVarChar, 150).Value = patient.Text.Trim();
                            cmd.Parameters.Add("@phone", SqlDbType.NVarChar, 40).Value = (object)phone.Text.Trim() ?? DBNull.Value;
                            cmd.Parameters.Add("@start", SqlDbType.DateTime2).Value = Start;
                            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = (int)duration.Value;
                            cmd.Parameters.Add("@notes", SqlDbType.NVarChar, 1000).Value = notes.Text.Trim();
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                }
                feedback.Text = T("نوبت ثبت شد.", "Appointment saved.");
                LoadRows();
            }
            catch (Exception ex) { ShowError(ex); }
        }
        void UpdateStatus()
        {
            if (!selectedId.HasValue) { feedback.Text = T("یک نوبت انتخاب کنید.", "Select an appointment."); return; }
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.AppointmentSlots SET Status=@status WHERE Id=@id", con))
                {
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar, 20).Value = status.SelectedItem.ToString();
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = selectedId.Value;
                    con.Open(); cmd.ExecuteNonQuery();
                }
                LoadRows();
            }
            catch (Exception ex) { ShowError(ex); }
        }
        void Cancel()
        {
            if (!selectedId.HasValue) { feedback.Text = T("یک نوبت انتخاب کنید.", "Select an appointment."); return; }
            status.SelectedItem = "Cancelled";
            UpdateStatus();
        }
        void LoadRows()
        {
            try
            {
                using (var con = DatabaseSettings.CreateConnection())
                using (var adapter = new SqlDataAdapter("SELECT TOP (300) Id,PatientName,Phone,AppointmentAt,DurationMinutes,Status,Notes FROM dbo.AppointmentSlots ORDER BY AppointmentAt DESC", con))
                {
                    var table = new DataTable(); adapter.Fill(table); grid.DataSource = table;
                }
                selectedId = null;
            }
            catch (Exception ex) { ShowError(ex); }
        }
        void ShowError(Exception ex)
        {
            feedback.Text = T("خطای پایگاه داده؛ ابتدا اسکریپت مرحله ۱۱ را اجرا کنید. ", "Database error; run Stage 11 migration first. ") + ex.Message;
        }
    }
}
