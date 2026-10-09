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
    public partial class frmParvandeh : Form
    {
        public frmParvandeh()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
            ClinicEntryVisuals.Attach(this, "پرونده بیمار", "Patient Record");
        }


        SqlConnection con = DatabaseSettings.CreateConnection();
        SqlCommand cmd = new SqlCommand();

        private void frmParvandeh_Load(object sender, EventArgs e)
        {
            System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();
            mskTarikh.Text = p.GetYear(DateTime.Now).ToString() + p.GetMonth(DateTime.Now).ToString("0#") + p.GetDayOfMonth(DateTime.Now).ToString("0#");
            mskTarikh1.Text = p.GetYear(DateTime.Now).ToString() + p.GetMonth(DateTime.Now).ToString("0#") + p.GetDayOfMonth(DateTime.Now).ToString("0#");
            mskTarikh2.Text = p.GetYear(DateTime.Now).ToString() + p.GetMonth(DateTime.Now).ToString("0#") + p.GetDayOfMonth(DateTime.Now).ToString("0#");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int code;
            if (!TryPatientCode(out code)) return;
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtNameKh.Text))
            {
                MessageBox.Show(this, ClinicLanguage.IsEnglish ? "Enter patient first and last name." : "نام و نام خانوادگی بیمار را وارد کنید.");
                return;
            }
            if (!AppointmentDatePicker.IsValidDate(mskTarikh.Text))
            {
                MessageBox.Show(this, ClinicLanguage.IsEnglish ? "Enter a valid registration date." : "تاریخ تشکیل پرونده معتبر نیست.");
                return;
            }
            const string sql = @"INSERT INTO dbo.Parvandeh
                (CodeP,Tarikh,NameBemar,NameKh,TarikhTavalod,Jensiyat,Tarikh1,Tarikh2,BemariF,BemariG,NameBemeh,MablaghBemeh,Tozih)
                VALUES (@code,@date,@first,@last,@birth,@gender,@visit,@next,@current,@previous,@insurance,@amount,@notes)";
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@code", SqlDbType.Int).Value = code;
                    command.Parameters.AddWithValue("@date", mskTarikh.Text);
                    command.Parameters.AddWithValue("@first", txtName.Text.Trim());
                    command.Parameters.AddWithValue("@last", txtNameKh.Text.Trim());
                    command.Parameters.AddWithValue("@birth", mskTarikhTavalod.Text);
                    command.Parameters.AddWithValue("@gender", cmbj.Text);
                    command.Parameters.AddWithValue("@visit", mskTarikh1.Text);
                    command.Parameters.AddWithValue("@next", mskTarikh2.Text);
                    command.Parameters.AddWithValue("@current", txtFeli.Text);
                    command.Parameters.AddWithValue("@previous", txtGabli.Text);
                    command.Parameters.AddWithValue("@insurance", txtNameBemeh.Text);
                    command.Parameters.AddWithValue("@amount", txtMablagh.Text);
                    command.Parameters.AddWithValue("@notes", txtTozih.Text);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                MessageBox.Show(this, ClinicLanguage.IsEnglish ? "Patient record saved." : "پرونده بیمار ثبت شد.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(this, ex.Number == 2627 || ex.Number == 2601
                    ? (ClinicLanguage.IsEnglish ? "This patient record number already exists." : "این شماره پرونده قبلاً ثبت شده است.")
                    : (ClinicLanguage.IsEnglish ? "Could not save patient record. Check data and SQL Server." : "ثبت پرونده انجام نشد. اطلاعات و اتصال دیتابیس را بررسی کنید."));
            }
        }

        private bool TryPatientCode(out int code)
        {
            if (int.TryParse(txtCode.Text.Trim(), out code) && code > 0)
            {
                errorProvider1.SetError(txtCode, "");
                return true;
            }
            errorProvider1.SetError(txtCode, ClinicLanguage.IsEnglish ? "Enter a valid positive patient ID." : "شماره پرونده معتبر وارد کنید.");
            txtCode.Focus();
            return false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int code;
            if (!TryPatientCode(out code)) return;
            var prompt = ClinicLanguage.IsEnglish ? "Delete this patient record? This cannot be undone." : "آیا از حذف پرونده بیمار اطمینان دارید؟ این عملیات قابل بازگشت نیست.";
            if (MessageBox.Show(this, prompt, ClinicLanguage.IsEnglish ? "Confirm deletion" : "تأیید حذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand("DELETE FROM dbo.Parvandeh WHERE CodeP=@code", connection))
                {
                    command.Parameters.Add("@code", SqlDbType.Int).Value = code;
                    connection.Open();
                    var affected = command.ExecuteNonQuery();
                    MessageBox.Show(this, affected > 0
                        ? (ClinicLanguage.IsEnglish ? "Patient record deleted." : "پرونده بیمار حذف شد.")
                        : (ClinicLanguage.IsEnglish ? "Patient record not found." : "پرونده‌ای با این شماره یافت نشد."));
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(this, ClinicLanguage.IsEnglish ? "Could not delete the patient record. Check linked records and database access." : "حذف پرونده انجام نشد. سوابق مرتبط و اتصال دیتابیس را بررسی کنید.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int code;
            if (!TryPatientCode(out code)) return;
            const string sql = @"UPDATE dbo.Parvandeh SET Tarikh=@date,NameBemar=@first,NameKh=@last,
                TarikhTavalod=@birth,Jensiyat=@gender,Tarikh1=@date1,Tarikh2=@date2,
                NameBemeh=@insurance,MablaghBemeh=@amount,BemariG=@previous,BemariF=@current,
                Tozih=@notes WHERE CodeP=@code";
            try
            {
                using (var connection = DatabaseSettings.CreateConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@code", code);
                    command.Parameters.AddWithValue("@date", mskTarikh.Text);
                    command.Parameters.AddWithValue("@first", txtName.Text);
                    command.Parameters.AddWithValue("@last", txtNameKh.Text);
                    command.Parameters.AddWithValue("@birth", mskTarikhTavalod.Text);
                    command.Parameters.AddWithValue("@gender", cmbj.Text);
                    command.Parameters.AddWithValue("@date1", mskTarikh1.Text);
                    command.Parameters.AddWithValue("@date2", mskTarikh2.Text);
                    command.Parameters.AddWithValue("@insurance", txtNameBemeh.Text);
                    command.Parameters.AddWithValue("@amount", txtMablagh.Text);
                    command.Parameters.AddWithValue("@previous", txtGabli.Text);
                    command.Parameters.AddWithValue("@current", txtFeli.Text);
                    command.Parameters.AddWithValue("@notes", txtTozih.Text);
                    connection.Open();
                    int changed = command.ExecuteNonQuery();
                    MessageBox.Show(this, changed > 0
                        ? (ClinicLanguage.IsEnglish ? "Patient record updated." : "پرونده بیمار ویرایش شد.")
                        : (ClinicLanguage.IsEnglish ? "Patient record not found." : "پرونده‌ای با این شماره یافت نشد."));
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(this, ClinicLanguage.IsEnglish ? "Update failed. Check the entered values and database connection." : "ویرایش انجام نشد. مقادیر و اتصال دیتابیس را بررسی کنید.");
            }
        }

        private void btnP_Click(object sender, EventArgs e)
        {
            if (txtCode.Text != "")
            {
                SqlDataReader dr;
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from Parvandeh where CodeP=@S";
                cmd.Parameters.AddWithValue("@S", txtCode.Text);
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtCode.Text = dr["CodeP"].ToString();
                    mskTarikh.Text = dr["Tarikh"].ToString();
                    txtName.Text = dr["NameBemar"].ToString();
                    txtNameKh.Text = dr["NameKh"].ToString();
                    mskTarikhTavalod.Text = dr["TarikhTavalod"].ToString();
                    cmbj.Text = dr["Jensiyat"].ToString();
                    mskTarikh1.Text = dr["Tarikh1"].ToString();
                    mskTarikh2.Text = dr["Tarikh2"].ToString();
                    txtGabli.Text = dr["BemariG"].ToString();
                    txtFeli.Text = dr["BemariF"].ToString();
                    txtNameBemeh.Text = dr["NameBemeh"].ToString();
                    txtMablagh.Text = dr["MablaghBemeh"].ToString();
                    txtTozih.Text = dr["Tozih"].ToString();

                }
                else
                {
                    txtCode.Text = "";
                    MessageBox.Show("ویزیتی با این کد وجود ندارد");
                }
                con.Close();
            }
        }

        private void btnBemeh_Click(object sender, EventArgs e)
        {
            if (txtCodeBemeh.Text != "")
            {
                SqlDataReader dr;
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from Bemeh where Id=@S";
                cmd.Parameters.AddWithValue("@S", txtCodeBemeh.Text);
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtNameBemeh.Text = dr["NameBemeh"].ToString();
                    txtMablagh.Text = dr["Mablagh"].ToString();
                }
                else
                {
                    txtCodeBemeh.Text = "";
                    MessageBox.Show("بیمه ای با این کد وجود ندارد");
                }
                con.Close();
            }
        }
    }
}
