using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopApp
{
    public partial class frmSetting : Form
    {
        public frmSetting()
        {
            InitializeComponent();
        }

        Color color;

        private void SelectColor(object senderBtn)
        {
            DisableColor();
            Button button = (Button)senderBtn;
            color = button.BackColor;
            button.Text = "Chọn";
        }

        private void DisableColor()
        {
            foreach (Control previouBtn in flowLayoutPanel1.Controls)
            {
                previouBtn.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SelectColor(sender);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmHome.color = color;
            btnSearch.BackColor = frmHome.color;
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            btnSearch.BackColor = frmHome.color;
            foreach (Control previouBtn in flowLayoutPanel1.Controls)
            {
                if(frmHome.color == previouBtn.BackColor)
                {
                    previouBtn.Text = "Đang dùng";
                }
            }
        }
    }
}
