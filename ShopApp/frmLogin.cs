using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopApp
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        public static string idLogin, nameLogin, positionLogin;

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Code.Functions.Disconnect();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            labelLogin.ForeColor = frmHome.color;
            btnLogin.BackColor = frmHome.color;
            CenterToScreen();
            lbPhoneError.Text = "";
            lbPasswordError.Text = "";
            Code.Functions.Connect();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtPhone.Text.Trim().Equals(""))
            {
                lbPhoneError.Text = "Số điện thoại không được để trống!";
                return;
            }
            else
            {
                lbPhoneError.Text = "";
            }

            if (!Code.Functions.IsPhoneNumber(txtPhone.Text.Trim()))
            {
                lbPhoneError.Text = "Số điện thoại không đúng định dạng!";
                return;
            }
            else
            {
                lbPhoneError.Text = "";
            }

            if (txtPassword.Text.Trim().Equals(""))
            {
                lbPasswordError.Text = "Mật khẩu không được để trống!";
                return;
            }
            else
            {
                lbPasswordError.Text = "";
            }

            try
            {
                SqlCommand cmd = Code.Functions.RunProcedure("LoginUser");

                cmd.Parameters.Add(new SqlParameter("@Phone", txtPhone.Text));
                cmd.Parameters.Add(new SqlParameter("@Password", Code.Functions.MD5Hash(txtPassword.Text.Trim())));

                cmd.ExecuteNonQuery();
                SqlDataReader data = cmd.ExecuteReader();

                if (data.Read())
                {
                    if (data["Position"].ToString().Equals("3"))
                    {
                        MessageBox.Show("Bạn không có quyền truy cập!");
                        data.Close();
                        cmd.Cancel();
                        return;
                    }

                    if (data["Status"].ToString().Equals("1"))
                    {

                        idLogin = data["Id"].ToString();
                        nameLogin = data["Name"].ToString();
                        positionLogin = data["Position"].ToString();
                        data.Close();
                        cmd.Cancel();
                        frmHome frm = new frmHome();
                        frm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Tài khoản này đã bị khóa");
                        data.Close();
                        cmd.Cancel();
                    }
                }
                else
                {
                    MessageBox.Show("Sai tên email hoặc mật khẩu, vui lòng kiểm tra lại");
                    data.Close();
                    cmd.Cancel();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi: "+ ex.Message);
            }
        }
    }
}
