using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Matab
{
    public partial class frmNobat : Form
    {
        public frmNobat()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
            ClinicEntryVisuals.Attach(this, "ثبت نوبت", "Book Appointment");
            AppointmentDatePicker.Attach(this, mskTarikh);
        }


        SqlConnection con = DatabaseSettings.CreateConnection();
        SqlCommand cmd = new SqlCommand();

        private void frmNobat_Load(object sender, EventArgs e)
        {
            System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();
            mskTarikh.Text = p.GetYear(DateTime.Now).ToString() + p.GetMonth(DateTime.Now).ToString("0#") + p.GetDayOfMonth(DateTime.Now).ToString("0#");
        }

        // Appointment numbers are globally unique in the legacy schema.
        // Check before writing; the database primary key remains the final safeguard.
        private bool AppointmentExists(string number)
        {
            int parsed;
            if (!int.TryParse(number, out parsed) || parsed <= 0)
            {
                MessageBox.Show(!ClinicLanguage.IsEnglish ? "شماره نوبت باید یک عدد مثبت باشد." : "Appointment number must be a positive integer.");
                return true;
            }
            using (var connection = DatabaseSettings.CreateConnection())
            using (var check = new SqlCommand("SELECT COUNT(1) FROM dbo.Nobat WHERE Nobat=@number", connection))
            {
                check.Parameters.Add("@number", SqlDbType.Int).Value = parsed;
                connection.Open();
                return Convert.ToInt32(check.ExecuteScalar()) > 0;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int number;
            if (!int.TryParse(txtNobat.Text.Trim(), out number) || number <= 0 || !AppointmentDatePicker.IsValidDate(mskTarikh.Text))
            {
                errorProvider1.SetError(txtNobat, ClinicLanguage.IsEnglish ? "Enter a positive appointment number and valid date." : "شماره نوبت مثبت و تاریخ معتبر وارد کنید.");
                return;
            }
            errorProvider1.SetError(txtNobat, "");
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtNameKh.Text))
            {
                MessageBox.Show(this, ClinicLanguage.IsEnglish ? "Enter patient first and last name." : "نام و نام خانوادگی بیمار را وارد کنید.");
                return;
            }
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand(@"INSERT INTO dbo.Nobat (Nobat,Tarikh,NameBemar,NameKh,Tel,Tozih)
                    VALUES (@number,@date,@first,@last,@phone,@notes)", connection))
                {
                    command.Parameters.Add("@number", SqlDbType.Int).Value = number;
                    command.Parameters.AddWithValue("@date", mskTarikh.Text);
                    command.Parameters.AddWithValue("@first", txtName.Text.Trim());
                    command.Parameters.AddWithValue("@last", txtNameKh.Text.Trim());
                    command.Parameters.AddWithValue("@phone", txtTel.Text.Trim());
                    command.Parameters.AddWithValue("@notes", txtTozih.Text);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                MessageBox.Show(this, ClinicLanguage.IsEnglish ? "Appointment booked." : "نوبت ثبت شد.");
                txtNobat.Text = "";
                txtName.Text = "";
                txtNameKh.Text = "";
                txtTel.Text = "";
                txtTozih.Text = "";
            }
            catch (SqlException ex)
            {
                MessageBox.Show(this, ex.Number == 2627 || ex.Number == 2601
                    ? (ClinicLanguage.IsEnglish ? "Appointment number already exists." : "این شماره نوبت قبلاً ثبت شده است.")
                    : (ClinicLanguage.IsEnglish ? "Could not save appointment. Check SQL Server." : "ثبت نوبت انجام نشد. اتصال دیتابیس را بررسی کنید."));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtNobat.Text == "")
            {
                errorProvider1.SetError(txtNobat, "شماره نوبت وارد نشده است");
            }
            else
            {
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "DELETE FROM Nobat WHERE Nobat=@number";
                cmd.Parameters.AddWithValue("@number", txtNobat.Text.Trim());
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("حذف وقت رزرو شده انجام شد");

                //************************************
                txtNobat.Text = "";
                mskTarikh.Text = "";
                txtName.Text = "";
                txtNameKh.Text = "";
                txtTel.Text = "";
                txtTozih.Text = "";
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtNobat.Text == "" || !AppointmentDatePicker.IsValidDate(mskTarikh.Text))
            {
                errorProvider1.SetError(txtNobat, !ClinicLanguage.IsEnglish ? "شماره نوبت یا تاریخ معتبر نیست." : "Invalid appointment number or date.");
            }
            else
            {
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE Nobat SET Tarikh=@date, NameBemar=@patient, NameKh=@family, Tel=@phone, Tozih=@notes WHERE Nobat=@number";
                cmd.Parameters.AddWithValue("@date", mskTarikh.Text);
                cmd.Parameters.AddWithValue("@patient", txtName.Text);
                cmd.Parameters.AddWithValue("@family", txtNameKh.Text);
                cmd.Parameters.AddWithValue("@phone", txtTel.Text);
                cmd.Parameters.AddWithValue("@notes", txtTozih.Text);
                cmd.Parameters.AddWithValue("@number", txtNobat.Text.Trim());
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("ویرایش وقت رزرو شده انجام شد");

                //************************************
                txtNobat.Text = "";
                mskTarikh.Text = "";
                txtName.Text = "";
                txtNameKh.Text = "";
                txtTel.Text = "";
                txtTozih.Text = "";
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (txtNobat.Text !="")
            {
                SqlDataReader dr;
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from Nobat where Nobat=@S";
                cmd.Parameters.AddWithValue("@S",txtNobat.Text);
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtNobat.Text = dr["Nobat"].ToString();
                    mskTarikh.Text = dr["Tarikh"].ToString();
                    txtName.Text = dr["NameBemar"].ToString();
                    txtNameKh.Text = dr["NameKh"].ToString();
                    txtTel.Text = dr["Tel"].ToString();
                    txtTozih.Text = dr["Tozih"].ToString();

                }
                else
                {
                    txtNobat.Text = "";
                    txtNobat.Focus();
                    MessageBox.Show("نوبتی با این شماره وجود ندارد");
                }
                con.Close();
            }
        }
    }
}
