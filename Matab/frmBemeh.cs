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
    public partial class frmBemeh : Form
    {
        public frmBemeh()
        {
            InitializeComponent();
            ClinicLanguage.Attach(this);
        }

        SqlConnection con = DatabaseSettings.CreateConnection();
        SqlCommand cmd = new SqlCommand();


        void Display()
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = new SqlCommand();
            adp.SelectCommand.Connection = con;
            adp.SelectCommand.CommandText = "select * from Bemeh";
            adp.Fill(ds, "Bemeh");
            dgvBemeh.DataSource = ds;
            dgvBemeh.DataMember = "Bemeh";
            dgvBemeh.Columns[0].Width = 50;
            dgvBemeh.Columns[0].HeaderText = "کد بیمه";
            dgvBemeh.Columns[1].HeaderText = "نام بیمه";
            dgvBemeh.Columns[2].HeaderText = "تعرفه بیمه";
            dgvBemeh.Columns[3].HeaderText = "توضیحات";
        }

        private void frmBemeh_Load(object sender, EventArgs e)
        {
            Display();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text=="" | txtMablagh.Text=="")
            {
                errorProvider1.SetError(txtName,"نام بیمه وارد نشده است");
                errorProvider1.SetError(txtMablagh, "تعرفه بیمه وارد نشده است");
            }
            else
            {
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "insert into Bemeh(NameBemeh,Mablagh,Tozih) Values(@a,@b,@c)";
                cmd.Parameters.AddWithValue("@a",txtName.Text);
                cmd.Parameters.AddWithValue("@b", txtMablagh.Text);
                cmd.Parameters.AddWithValue("@c",txtTozih.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                Display();
                MessageBox.Show("ثبت با موفقیت انجام شد");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtMablagh.Text == "")
            {
                errorProvider1.SetError(txtMablagh, "تعرفه بیمه وارد نشده است");
            }
            else
            {
                int x = Convert.ToInt32(dgvBemeh.SelectedCells[0].Value);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Delete from Bemeh where Id=@N";
                cmd.Parameters.AddWithValue("@N",x);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                Display();
                MessageBox.Show("حذف با موفقیت انجام شد");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtMablagh.Text == "")
            {
                errorProvider1.SetError(txtMablagh, "تعرفه بیمه وارد نشده است");
            }
            else
            {
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Update Bemeh set NameBemeh='"+txtName.Text+"',Mablagh='"+txtMablagh.Text+"',Tozih='"+txtTozih.Text+"' where Id="+ txtCode.Text;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                Display();
                MessageBox.Show("ویرایش با موفقیت انجام شد");
            }
        }

        private void dgvBemeh_MouseUp(object sender, MouseEventArgs e)
        {
            txtCode.Text = dgvBemeh[0, dgvBemeh.CurrentRow.Index].Value.ToString() ;
            txtName.Text = dgvBemeh[1, dgvBemeh.CurrentRow.Index].Value.ToString();
            txtMablagh.Text = dgvBemeh[2, dgvBemeh.CurrentRow.Index].Value.ToString();
            txtTozih.Text = dgvBemeh[3, dgvBemeh.CurrentRow.Index].Value.ToString();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            NativeReport Report = new NativeReport(this);
            Report.Load("Report/rptBemeh.mrt");
            Report.Compile();
            Report.ShowWithRibbonGUI();
        }
    }
}
