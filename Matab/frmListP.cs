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
    public partial class frmListP : Form
    {
        public frmListP()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
            ConfigureGrid();
            ClinicListVisuals.Apply(this, dgvParvandeh);
        }


        SqlConnection con = DatabaseSettings.CreateConnection();
        SqlCommand cmd = new SqlCommand();

        private void ConfigureGrid()
        {
            dgvParvandeh.ReadOnly = true;
            dgvParvandeh.AllowUserToAddRows = false;
            dgvParvandeh.AllowUserToDeleteRows = false;
            dgvParvandeh.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvParvandeh.MultiSelect = false;
            dgvParvandeh.RowHeadersVisible = false;
            dgvParvandeh.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvParvandeh.BackgroundColor = Color.White;
            dgvParvandeh.BorderStyle = BorderStyle.None;
            dgvParvandeh.EnableHeadersVisualStyles = false;
            dgvParvandeh.ColumnHeadersHeight = 40;
            dgvParvandeh.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(225, 241, 245);
            dgvParvandeh.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(16, 48, 69);
            dgvParvandeh.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 251);
            dgvParvandeh.DefaultCellStyle.SelectionBackColor = Color.FromArgb(20, 123, 140);
            dgvParvandeh.RowTemplate.Height = 32;
        }

        private void Display(string field = null, string search = null)
        {
            string sql = "SELECT * FROM dbo.Parvandeh";
            if (!string.IsNullOrEmpty(field) && !string.IsNullOrWhiteSpace(search))
                sql += " WHERE " + field + " LIKE @Search";
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    if (sql.Contains("@Search"))
                        command.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = "%" + search.Trim() + "%";
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var data = new DataTable();
                        adapter.Fill(data);
                        dgvParvandeh.DataSource = data;
                        string[] fa = {"ردیف", "شماره پرونده", "تاریخ تشکیل پرونده", "نام بیمار", "نام خانوادگی", "تاریخ تولد", "جنسیت", "تاریخ مراجعه فعلی", "تاریخ مراجعه بعدی", "بیماری فعلی", "بیماری قبلی", "نام بیمه", "تعرفه بیمه", "دستور پزشک"};
                        string[] en = {"Row", "File No.", "Registration date", "Patient name", "Last name", "Birth date", "Gender", "Current visit", "Next visit", "Current condition", "Medical history", "Insurance", "Insurance tariff", "Doctor instructions"};
                        for (int i = 0; i < dgvParvandeh.Columns.Count && i < fa.Length; i++)
                            dgvParvandeh.Columns[i].HeaderText = ClinicLanguage.IsEnglish ? en[i] : fa[i];
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(ClinicLanguage.IsEnglish ? "Could not load records. Check SQL Server." : "بارگذاری اطلاعات ممکن نیست. اتصال SQL Server را بررسی کنید.");
            }
        }

        private void frmListP_Load(object sender, EventArgs e)
        {
            Display();
        }

        private void txtNameKh_TextChanged(object sender, EventArgs e)
        {
            Display("NameKh", txtNameKh.Text);
        }

        private void mskTarikh_TextChanged(object sender, EventArgs e)
        {
            Display("Tarikh2", mskTarikh.Text);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            NativeReport Report = new NativeReport(this);
            Report.Load("Report/rptParvandeh.mrt");
            Report.Compile();
            Report["strTarikh"] = mskTarikh.Text;
            Report.ShowWithRibbonGUI();
        }
    }
}
