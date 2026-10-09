using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Matab.Reporting;
using System.Drawing.Printing;

namespace Matab
{
    public partial class frmGavahi : Form
    {
        private Button previewCertificateButton;
        private Button printCertificateButton;

        public frmGavahi()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
            ConfigureCertificateActions();
        }

        private void frmGavahi_Load(object sender, EventArgs e)
        {
            var calendar = new System.Globalization.PersianCalendar();
            var today = DateTime.Now;
            mskTarikh1.Text = string.Format("{0:0000}{1:00}{2:00}",
                calendar.GetYear(today), calendar.GetMonth(today), calendar.GetDayOfMonth(today));

            // A fresh connection per operation avoids leaking open connections.
            // Setting may be empty on a newly installed clinic database.
            lblName.Text = ClinicBrand.DoctorName;
            lblTel.Text = "";
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand("SELECT TOP (1) NameMatab, Tel FROM Setting ORDER BY id", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            lblName.Text = reader.IsDBNull(0) ? lblName.Text : Convert.ToString(reader.GetValue(0));
                            lblTel.Text = reader.IsDBNull(1) ? "" : Convert.ToString(reader.GetValue(1));
                        }
                        else
                        {
                            MessageBox.Show(
                                ClinicLanguage.IsEnglish
                                    ? "Clinic information is not configured. Please complete Settings first."
                                    : "اطلاعات مطب هنوز ثبت نشده است. ابتدا بخش تنظیمات را تکمیل کنید.",
                                ClinicLanguage.IsEnglish ? "Clinic settings" : "تنظیمات مطب",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    ClinicLanguage.IsEnglish ? "Unable to load clinic details from SQL Server. Check the database connection."
                        : "دریافت اطلاعات مطب از SQL Server ممکن نشد. اتصال دیتابیس را بررسی کنید.",
                    ClinicLanguage.IsEnglish ? "Database error" : "خطای پایگاه داده",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (System.Configuration.ConfigurationErrorsException)
            {
                MessageBox.Show(
                    ClinicLanguage.IsEnglish ? "Database connection settings are missing or invalid."
                        : "تنظیمات اتصال پایگاه داده موجود نیست یا معتبر نیست.",
                    ClinicLanguage.IsEnglish ? "Configuration error" : "خطای تنظیمات",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigureCertificateActions()
        {
            // Keep legacy designer controls intact; add safe print actions dynamically.
            previewCertificateButton = new Button { Width = 180, Height = 44, BackColor = Color.FromArgb(0, 133, 150), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            printCertificateButton = new Button { Width = 160, Height = 44, BackColor = Color.FromArgb(17, 63, 91), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            previewCertificateButton.FlatAppearance.BorderSize = 0;
            printCertificateButton.FlatAppearance.BorderSize = 0;
            var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 72, Padding = new Padding(12), FlowDirection = FlowDirection.LeftToRight, BackColor = Color.FromArgb(235, 245, 249) };
            previewCertificateButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            printCertificateButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            actions.Controls.Add(previewCertificateButton);
            actions.Controls.Add(printCertificateButton);
            Controls.Add(actions);
            actions.BringToFront();
            previewCertificateButton.Click += (sender, args) => ShowCertificate(false);
            printCertificateButton.Click += (sender, args) => ShowCertificate(true);
            Activated += (sender, args) => UpdateCertificateLabels();
            BackColor = Color.FromArgb(247, 251, 253);
            UpdateCertificateLabels();
        }

        private void UpdateCertificateLabels()
        {
            previewCertificateButton.Text = ClinicLanguage.IsEnglish ? "Preview certificate" : "پیش نمایش گواهی";
            printCertificateButton.Text = ClinicLanguage.IsEnglish ? "Print certificate" : "چاپ گواهی";
        }

        private void ShowCertificate(bool print)
        {
            UpdateCertificateLabels();
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(ClinicLanguage.IsEnglish ? "Enter the patient's full name." : "نام و نام خانوادگی بیمار را وارد کنید.");
                txtName.Focus();
                return;
            }
            var model = new CertificatePrintModel
            {
                Patient = txtName.Text.Trim(),
                Reason = txtBemari.Text.Trim(),
                Location = txtMakan.Text.Trim(),
                Days = Math.Max(1, txtTedad.Value),
                Date = mskTarikh1.Text.Trim(),
                Phone = lblTel.Text.Trim(),
                English = ClinicLanguage.IsEnglish
            };
            using (var document = MedicalCertificatePrinter.Create(model))
            {
                try
                {
                    if (print)
                    {
                        using (var dialog = new PrintDialog { Document = document, UseEXDialog = true })
                            if (dialog.ShowDialog(this) == DialogResult.OK) document.Print();
                    }
                    else
                    {
                        using (var preview = new PrintPreviewDialog { Document = document, Width = 1000, Height = 760, StartPosition = FormStartPosition.CenterParent })
                            preview.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show((ClinicLanguage.IsEnglish ? "Unable to print certificate: " : "خطا در چاپ گواهی: ") + ex.Message,
                        ClinicLanguage.IsEnglish ? "Print error" : "خطای چاپ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Existing designer button now opens the modern bilingual preview.
            ShowCertificate(false);
        }
    }
}
