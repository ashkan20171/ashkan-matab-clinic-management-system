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
    public partial class frmListNobat : Form
    {
        public frmListNobat()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
            ConfigureGrid();
            ClinicListVisuals.Apply(this, dgvNobat);
        }

        SqlConnection con = DatabaseSettings.CreateConnection();
        SqlCommand cmd = new SqlCommand();

        private void ConfigureGrid()
        {
            dgvNobat.ReadOnly = true;
            dgvNobat.AllowUserToAddRows = false;
            dgvNobat.AllowUserToDeleteRows = false;
            dgvNobat.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNobat.MultiSelect = false;
            dgvNobat.RowHeadersVisible = false;
            dgvNobat.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNobat.BackgroundColor = Color.White;
            dgvNobat.BorderStyle = BorderStyle.None;
            dgvNobat.EnableHeadersVisualStyles = false;
            dgvNobat.ColumnHeadersHeight = 40;
            dgvNobat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(225, 241, 245);
            dgvNobat.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(16, 48, 69);
            dgvNobat.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 251);
            dgvNobat.DefaultCellStyle.SelectionBackColor = Color.FromArgb(20, 123, 140);
            dgvNobat.RowTemplate.Height = 32;
        }

        private void Display(string field = null, string search = null)
        {
            string sql = "SELECT * FROM dbo.Nobat";
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
                        dgvNobat.DataSource = data;
                        string[] fa = {"ردیف", "شماره نوبت", "تاریخ", "نام", "نام خانوادگی", "تلفن تماس", "توضیحات"};
                        string[] en = {"Row", "Appointment No.", "Date", "First name", "Last name", "Phone", "Notes"};
                        for (int i = 0; i < dgvNobat.Columns.Count && i < fa.Length; i++)
                            dgvNobat.Columns[i].HeaderText = ClinicLanguage.IsEnglish ? en[i] : fa[i];
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(ClinicLanguage.IsEnglish ? "Could not load records. Check SQL Server." : "بارگذاری اطلاعات ممکن نیست. اتصال SQL Server را بررسی کنید.");
            }
        }

        private void frmListNobat_Load(object sender, EventArgs e)
        {
            Display();
        }

        private void txtNameKh_TextChanged(object sender, EventArgs e)
        {
            Display("NameKh", txtNameKh.Text);
        }

        private void txtNobat_TextChanged(object sender, EventArgs e)
        {
            Display("Nobat", txtNobat.Text);
        }

        private void mskTarikh_TextChanged(object sender, EventArgs e)
        {
            Display("Tarikh", mskTarikh.Text);
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (dgvNobat.CurrentRow == null || !dgvNobat.Columns.Contains("Id"))
            {
                MessageBox.Show(ClinicLanguage.IsEnglish ? "Select an appointment first." : "ابتدا یک نوبت انتخاب کنید.");
                return;
            }
            int id;
            if (!int.TryParse(Convert.ToString(dgvNobat.CurrentRow.Cells["Id"].Value), out id))
            {
                MessageBox.Show(ClinicLanguage.IsEnglish ? "Invalid appointment ID." : "شناسه نوبت نامعتبر است.");
                return;
            }
            var question = ClinicLanguage.IsEnglish ? "Delete the selected appointment?" : "آیا از حذف نوبت انتخاب‌شده اطمینان دارید؟";
            if (MessageBox.Show(question, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand("DELETE FROM dbo.Nobat WHERE Id = @Id", connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    connection.Open();
                    int changed = command.ExecuteNonQuery();
                    if (changed == 0) MessageBox.Show(ClinicLanguage.IsEnglish ? "Appointment was not found." : "نوبت موردنظر پیدا نشد.");
                }
                Display();
            }
            catch (SqlException)
            {
                MessageBox.Show(ClinicLanguage.IsEnglish ? "Could not delete the appointment." : "حذف نوبت امکان‌پذیر نیست.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            NativeReport Report = new NativeReport(this);
            Report.Load("Report/rptNobat.mrt");
            Report.Compile();
            Report["strTarikh1"]=mskTarikh.Text;
            Report["strTarikh2"] = mskTarikh.Text;
            Report.ShowWithRibbonGUI();
        }
    }
}
