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
using COMExcel = Microsoft.Office.Interop.Excel;

namespace ShopApp
{
    public partial class frmDetailOrder : Form
    {
        public frmDetailOrder()
        {
            InitializeComponent();
        }

        string idOrder, idPersonel, idCustommer, total;

        private void LoadDataGridView()
        {
            SqlCommand cmd = Functions.RunProcedure("GetOrderProducts");
            cmd.Parameters.Add(new SqlParameter("@Id_order", idOrder));

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();

            SqlDataAdapter dap = new SqlDataAdapter(cmd);

            dap.Fill(dt);
            dgvOrderProduct.AutoGenerateColumns = false;
            dgvOrderProduct.DataSource = dt;
            dgvOrderProduct.Refresh();
            cmd.Cancel();
        }

        private void frmDetailOrder_Load(object sender, EventArgs e)
        {
            btnPrint.BackColor = frmHome.color;
            button2.BackColor = frmHome.color;
            CenterToScreen();
            LoadDataGridView();
            lbSumMoney.Text = Functions.FormatMoney(total);

            SqlCommand cmd = Functions.RunProcedure("GetUserId");
            cmd.Parameters.Add(new SqlParameter("@Id", idCustommer));
            cmd.ExecuteNonQuery();
            SqlDataReader data = cmd.ExecuteReader();
            if (data.Read())
            {
                lbIdUser.Text = data["Id"].ToString();
                lbNameUser.Text = data["Name"].ToString();
                lbEmailUser.Text = data["Email"].ToString();
                lbPhoneUser.Text = data["Phone"].ToString();
                lbAddressUser.Text = data["Address"].ToString();
                data.Close();
                cmd.Cancel();
            }
            else
            {
                MessageBox.Show("Khách hàng này đã bị xóa!");
                Close();
                data.Close();
                cmd.Cancel();
            }

            SqlCommand cmdp = Functions.RunProcedure("GetUserId");
            cmdp.Parameters.Add(new SqlParameter("@Id", idPersonel));
            cmdp.ExecuteNonQuery();
            SqlDataReader datap = cmdp.ExecuteReader();
            if (datap.Read())
            {
                lbPersonelName.Text = datap["Name"].ToString();
                cmdp.Cancel();
                datap.Close();
            }
            else
            {
                datap.Close();
                cmdp.Cancel();
                MessageBox.Show("Nhân viên này đã bị xóa!");
                Close();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // Khởi động chương trình Excel
            COMExcel.Application exApp = new COMExcel.Application();
            COMExcel.Workbook exBook; //Trong 1 chương trình Excel có nhiều Workbook
            COMExcel.Worksheet exSheet; //Trong 1 Workbook có nhiều Worksheet
            COMExcel.Range exRange;
            int hang = 0, cot = 0;
            exBook = exApp.Workbooks.Add(COMExcel.XlWBATemplate.xlWBATWorksheet);
            exSheet = exBook.Worksheets[1];
            // Định dạng chung
            exRange = exSheet.Cells[1, 1];
            exRange.Range["A1:Z300"].Font.Name = "Times new roman"; //Font chữ
            exRange.Range["A1:B3"].Font.Size = 10;
            exRange.Range["A1:B3"].Font.Bold = true;
            exRange.Range["A1:B3"].Font.ColorIndex = 5; //Màu xanh da trời
            exRange.Range["A1:A1"].ColumnWidth = 7;
            exRange.Range["B1:B1"].ColumnWidth = 15;
            exRange.Range["A1:B1"].MergeCells = true;
            exRange.Range["A1:B1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A1:B1"].Value = "Shop App";
            exRange.Range["A2:B2"].MergeCells = true;
            exRange.Range["A2:B2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A2:B2"].Value = "Trâu Quỳ - Gia Lâm - Hà Nội";
            exRange.Range["A3:B3"].MergeCells = true;
            exRange.Range["A3:B3"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A3:B3"].Value = "Điện thoại: 0987654321";
            exRange.Range["C2:E2"].Font.Size = 16;
            exRange.Range["C2:E2"].Font.Bold = true;
            exRange.Range["C2:E2"].Font.ColorIndex = 3; //Màu đỏ
            exRange.Range["C2:E2"].MergeCells = true;
            exRange.Range["C2:E2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["C2:E2"].Value = "HÓA ĐƠN BÁN";
            // Biểu diễn thông tin chung của hóa đơn bán

            exRange.Range["B6:C9"].Font.Size = 12;
            exRange.Range["B6:B6"].Value = "Mã hóa đơn:";
            exRange.Range["C6:E6"].MergeCells = true;
            exRange.Range["C6:E6"].Value = idOrder.ToString();
            exRange.Range["B7:B7"].Value = "Tên khách hàng:";
            exRange.Range["C7:E7"].MergeCells = true;
            exRange.Range["C7:E7"].Value = lbNameUser.Text.ToString();
            exRange.Range["B8:B8"].Value = "Địa chỉ:";
            exRange.Range["C8:E8"].MergeCells = true;
            exRange.Range["C8:E8"].Value = lbAddressUser.Text.ToString();
            exRange.Range["B9:B9"].Value = "Điện thoại:";
            exRange.Range["C9:E9"].MergeCells = true;
            exRange.Range["C9:E9"].Value = "'"+lbPhoneUser.Text.ToString();
            exRange.Range["B10:B10"].Value = "Email:";
            exRange.Range["C10:E10"].MergeCells = true;
            exRange.Range["C10:E10"].Value = lbEmailUser.Text.ToString();
            //Tạo dòng tiêu đề bảng
            exRange.Range["A11:G11"].Font.Bold = true;
            exRange.Range["A11:F11"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["C11:F11"].ColumnWidth = 15;
            exRange.Range["A11:A11"].Value = "STT";
            exRange.Range["B11:B11"].Value = "Mã SP";
            exRange.Range["C11:C11"].Value = "Tên SP";
            exRange.Range["D11:D11"].Value = "Số lượng";
            exRange.Range["E11:E11"].Value = "Giá bán";
            exRange.Range["F11:F11"].Value = "Thành tiền";
            exRange.Range["G11:G11"].ColumnWidth = 20;
            exRange.Range["G11:G11"].Value = "Ngày bán";
            for (hang = 0; hang < dgvOrderProduct.RowCount; hang++)
            {
                exSheet.Cells[1][hang + 12] = hang + 1;
                for (cot = 0; cot < dgvOrderProduct.ColumnCount; cot++)
                {
                    exSheet.Cells[cot + 2][hang + 12] = dgvOrderProduct.Rows[hang].Cells[cot].Value.ToString();
                    if (cot == 3) exSheet.Cells[cot + 2][hang + 12] = Functions.FormatMoney(dgvOrderProduct.Rows[hang].Cells[cot].Value.ToString()).ToString() + "đ";
                    if (cot == 4) exSheet.Cells[cot + 2][hang + 12] = Functions.FormatMoney(dgvOrderProduct.Rows[hang].Cells[cot].Value.ToString()).ToString() + "đ";
                }
            }
            exRange = exSheet.Cells[cot][hang + 14];
            exRange.Font.Bold = true;
            exRange.Value2 = "Tổng tiền:";
            exRange = exSheet.Cells[cot + 1][hang + 14];
            exRange.Font.Bold = true;
            exRange.Value2 = Functions.FormatMoney(total).ToString()+" VND";
            exRange = exSheet.Cells[4][hang + 17]; //Ô A1 
            exRange.Range["A1:C1"].MergeCells = true;
            exRange.Range["A1:C1"].Font.Italic = true;
            exRange.Range["A1:C1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            DateTime d = Convert.ToDateTime(DateTime.Now.Date.ToString("MM/dd/yyyy"));
            exRange.Range["A1:C1"].Value = "Hà Nội, ngày " + d.Day + " tháng " + d.Month + " năm " + d.Year;
            exRange.Range["A2:C2"].MergeCells = true;
            exRange.Range["A2:C2"].Font.Italic = true;
            exRange.Range["A2:C2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A2:C2"].Value = "Nhân viên bán hàng";
            exRange.Range["A6:C6"].MergeCells = true;
            exRange.Range["A6:C6"].Font.Italic = true;
            exRange.Range["A6:C6"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A6:C6"].Value = lbPersonelName.Text;
            exSheet.Name = "Hóa đơn bán hàng";
            exApp.Visible = true;
        }

        public void DataFormChill(string idOrder, string idPersonel, string idCustommer, string total)
        {
            this.idOrder = idOrder;
            this.idPersonel = idPersonel;
            this.idCustommer = idCustommer;
            this.total = total;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
