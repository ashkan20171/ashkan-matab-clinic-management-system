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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
            ConfigureLoginExperience();
        }

        // Presentation-only enhancement: leaves the legacy designer and authentication intact.
        private void ConfigureLoginExperience()
        {
            SuspendLayout();
            BackColor = Color.FromArgb(239, 247, 250);
            ClientSize = new Size(491, 268);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;
            Text = "Matab | Clinic Management";
            groupPanel1.Size = new Size(491, 268);
            groupPanel1.BackColor = Color.FromArgb(239, 247, 250);
            txtPassword.UseSystemPasswordChar = true;
            btnopen.BackColor = Color.FromArgb(13, 133, 151);
            btnClose.BackColor = Color.FromArgb(230, 239, 243);
            var heading = new Label
            {
                Name = "matabLoginHeading", AutoSize = false,
                Location = new Point(20, 198), Size = new Size(450, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 65, 82),
                Text = ClinicLanguage.IsEnglish ? "Welcome to Matab" : "به سامانه مدیریت مطب خوش آمدید"
            };
            var hint = new Label
            {
                Name = "matabLoginHint", AutoSize = false,
                Location = new Point(20, 226), Size = new Size(450, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 8F),
                ForeColor = Color.FromArgb(89, 112, 123),
                Text = ClinicLanguage.IsEnglish ? "Secure clinic access" : "ورود امن به سامانه مطب"
            };
            groupPanel1.Controls.Add(heading);
            groupPanel1.Controls.Add(hint);
            ResumeLayout(false);
        }

        SqlConnection con = DatabaseSettings.CreateConnection();
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();

        private void btnopen_Click(object sender, EventArgs e)
        {
            string struser;

            if (string.Equals(Convert.ToString(cmbAccess.SelectedItem), "مدیر", StringComparison.Ordinal) ||
                string.Equals(Convert.ToString(cmbAccess.SelectedItem), "Administrator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Convert.ToString(cmbAccess.SelectedItem), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                struser = "admin";
                clc_variable.stru = "مدیر";
            }

            else 
            {
                clc_variable.stru = "کاربر عادی";
                struser = "user";
            }
            using (var connection = DatabaseSettings.CreateConnection())
            using (var command = new SqlCommand("SELECT Password FROM Karbar WHERE Access=@access AND UserName=@username", connection))
            {
                command.Parameters.Add("@access", SqlDbType.NVarChar, 50).Value = struser;
                command.Parameters.Add("@username", SqlDbType.NVarChar, 256).Value = txtUserName.Text.Trim();
                try
                {
                    connection.Open();
                    var stored = command.ExecuteScalar() as string;
                    if (PasswordSecurity.Verify(txtPassword.Text, stored))
                    {
                        this.Hide();
                        using (var main = new Form1()) main.ShowDialog(this);
                        this.Close();
                    }
                    else MessageBox.Show(ClinicLanguage.IsEnglish ? "Invalid username or password." : "نام کاربری یا رمز عبور صحیح نیست.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("ارتباط با پایگاه داده برقرار نشد: " + ex.Message);
                }
            }
        }
    }
}
