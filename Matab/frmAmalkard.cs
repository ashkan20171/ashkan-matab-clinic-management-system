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

namespace Matab
{
    public partial class frmAmalkard : Form
    {
        public frmAmalkard()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
        }

        SqlConnection con = DatabaseSettings.CreateConnection();
        SqlCommand cmd = new SqlCommand();

        private void frmAmalkard_Load(object sender, EventArgs e)
        {
            System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();
            mskTarikh1.Text = p.GetYear(DateTime.Now).ToString() + p.GetMonth(DateTime.Now).ToString("0#") + p.GetDayOfMonth(DateTime.Now).ToString("0#");
            mskTarikh2.Text = p.GetYear(DateTime.Now).ToString() + p.GetMonth(DateTime.Now).ToString("0#") + p.GetDayOfMonth(DateTime.Now).ToString("0#");

            lblName.Text = ClinicBrand.DoctorName;
            lblTel.Text = "";
            try
            {
                using (var settingsConnection = DatabaseSettings.CreateConnection())
                using (var settingsCommand = new SqlCommand("SELECT TOP (1) NameMatab, Tel FROM Setting ORDER BY id", settingsConnection))
                {
                    settingsConnection.Open();
                    using (var reader = settingsCommand.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(0)) lblName.Text = Convert.ToString(reader.GetValue(0));
                            if (!reader.IsDBNull(1)) lblTel.Text = Convert.ToString(reader.GetValue(1));
                        }
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(ClinicLanguage.IsEnglish
                    ? "Clinic settings could not be loaded from SQL Server."
                    : "بارگذاری اطلاعات مطب از پایگاه داده امکان‌پذیر نیست.",
                    ClinicLanguage.IsEnglish ? "Database error" : "خطای پایگاه داده",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            RefreshTotals();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            RefreshTotals();
        }

        private void RefreshTotals()
        {
            // One parameterized aggregate avoids NULL casts and SQL injection.
            const string query = @"SELECT COUNT(*), COALESCE(SUM(CAST(MablaghBemeh AS decimal(18,2))),0),
                COALESCE(SUM(CAST(MablaghKhadamat AS decimal(18,2))),0),
                COALESCE(SUM(CAST(Mablagh AS decimal(18,2))),0)
                FROM Vizit WHERE Tarikh BETWEEN @start AND @end";
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@start", SqlDbType.NVarChar, 20).Value = mskTarikh1.Text.Trim();
                    command.Parameters.Add("@end", SqlDbType.NVarChar, 20).Value = mskTarikh2.Text.Trim();
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read()) return;
                        lblTedad.Text = Convert.ToString(reader.GetValue(0));
                        lblBemeh.Text = Convert.ToString(reader.GetValue(1));
                        lblKhadamat.Text = Convert.ToString(reader.GetValue(2));
                        lblMablagh.Text = Convert.ToString(reader.GetValue(3));
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show((ClinicLanguage.IsEnglish ? "Unable to load financial report: " : "خطا در بارگذاری گزارش مالی: ") + ex.Message,
                    ClinicLanguage.IsEnglish ? "Report error" : "خطای گزارش", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            NativeReport Report = new NativeReport(this);
            Report.Load("Report/rptAmalkard.mrt");
            Report.Compile();
            Report["strTarikh1"] = mskTarikh1.Text;
            Report["strTarikh2"] = mskTarikh2.Text;
            Report["Tedad"] = lblTedad.Text;
            Report["Bemeh"] = lblBemeh.Text;
            Report["Khadamat"] = lblKhadamat.Text;
            Report["MablaghKol"] = lblMablagh.Text;

            Report["NameMatab"] = lblName.Text;
            Report["Tel"] = lblTel.Text;

            Report.ShowWithRibbonGUI();
        }
    }
}
