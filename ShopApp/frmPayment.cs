using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShopApp.Code;

namespace ShopApp
{
    public partial class frmPayment : Form
    {
        public frmPayment()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmCarts.isClosePayment = true;
            Close();
        }

        Int64 data;

        public void Payment(Int64 value)
        {
            data = value;
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (txtPayment.Text.Trim().Equals(""))
            {
                lbPaymentError.Text = "Không được để trống!";
                return;
            }
            else
            {
                lbPaymentError.Text = "";
            }

            if (!int.TryParse(txtPayment.Text.Trim(), out int k))
            {
                lbPaymentError.Text = "Không được chứa kí tự!";
                return;
            }
            else
            {
                lbPaymentError.Text = "";
            }

            if (Int64.Parse(txtPayment.Text) < data)
            {
                MessageBox.Show("Số tiền khách đưa chưa đủ!");
            }
            else
            {
                Int64 money = Int64.Parse(txtPayment.Text) - data;

                MessageBox.Show("Trả lại tiền cho khách hàng " + Functions.FormatMoney(money.ToString()) + "đ");
                frmCarts.isClosePayment = false;
                Close();
            }
        }

        private void frmPayment_Load(object sender, EventArgs e)
        {
            btnPayment.BackColor = frmHome.color;
            button1.BackColor = frmHome.color;
            CenterToScreen();
            lbPaymentError.Text = "";
            txtPayment.Select();
        }
    }
}
