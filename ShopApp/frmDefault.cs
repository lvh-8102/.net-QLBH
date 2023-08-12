using ShopApp.Code;
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
    public partial class frmDefault : Form
    {
        public frmDefault()
        {
            InitializeComponent();
        }
        private void frmDefault_Load(object sender, EventArgs e)
        {
            lbName.ForeColor = frmHome.color;
            panel1.BackColor = frmHome.color;
            panel2.BackColor = frmHome.color;
            lbName.Text = frmLogin.nameLogin;
            SqlCommand cmd = Code.Functions.RunProcedure("CountOrders");
            cmd.ExecuteNonQuery();
            SqlDataReader data = cmd.ExecuteReader();

            if (data.Read())
            {
                lbOrder.Text = Functions.FormatMoney(data[0].ToString());
            }
            cmd.Cancel();
            data.Close();

            SqlCommand cmdc = Code.Functions.RunProcedure("SumOrders");
            cmdc.ExecuteNonQuery();
            SqlDataReader datac = cmdc.ExecuteReader();

            if (datac.Read())
            {
                lbMoney.Text = Functions.FormatMoney(datac[0].ToString()) + "đ";
            }
            cmdc.Cancel();
            datac.Close();
        }
    }
}
