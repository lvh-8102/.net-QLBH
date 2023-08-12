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
    public partial class frmCarts : Form
    {
        public frmCarts()
        {
            InitializeComponent();
        }

        string userID = frmLogin.idLogin;
        string totalMoney;

        List<Product> quantiyEdit = new List<Product>();

        private void LoadDataGridView(string name, DataGridView table)
        {
            DataTable dt = Functions.GetData(name);
            table.AutoGenerateColumns = false;
            table.DataSource = dt;
            table.Refresh();
        }

        private void frmCarts_Load(object sender, EventArgs e)
        {
            btnSearchUsers.BackColor = frmHome.color;
            btnSearchProducts.BackColor = frmHome.color;
            btnCreateCart.BackColor = frmHome.color;
            LoadDataGridView("GetCustomerUsers", dgvUsers);
            LoadDataGridView("GetProducts", dgvProducts);
            for (int i = 0; i < dgvProducts.RowCount; i++)
            {
                DataGridViewRow dgvRow = dgvProducts.Rows[i];
                Product pr = new Product();
                pr.Id = dgvRow.Cells["Idp"].Value.ToString();
                pr.Quantity = dgvRow.Cells["Quantity"].Value.ToString();

                quantiyEdit.Add(pr);
            }
        }

        private void btnSearchUsers_Click(object sender, EventArgs e)
        {
            string searchId = "", searchEmail = "", searchPhone = "";

            SqlCommand cmd = Functions.RunProcedure("SearchCustommerUsers");
            if (Functions.IsPhoneNumber(txtSearchUsers.Text.Trim()))
            {
                searchPhone = txtSearchUsers.Text;
            }
            else if (Code.Functions.IsValidEmail(txtSearchUsers.Text.Trim()))
            {
                searchEmail = txtSearchUsers.Text;
            }
            else if (int.TryParse(txtSearchUsers.Text, out int i))
            {
                searchId = txtSearchUsers.Text;
            }
            cmd.Parameters.Add(new SqlParameter("@Id", searchId));
            cmd.Parameters.Add(new SqlParameter("@Name", txtSearchUsers.Text));
            cmd.Parameters.Add(new SqlParameter("@Email", searchEmail));
            cmd.Parameters.Add(new SqlParameter("@Phone", searchPhone));

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();

            SqlDataAdapter dap = new SqlDataAdapter(cmd);

            dap.Fill(dt);
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.DataSource = dt;
            dgvUsers.Refresh();
            cmd.Cancel();
        }

        private void btnSearchProducts_Click(object sender, EventArgs e)
        {
            string searchId = "";

            SqlCommand cmd = Functions.RunProcedure("SearchProducts");
            if (int.TryParse(txtSearchProducts.Text, out int i))
            {
                searchId = txtSearchProducts.Text;
            }
            cmd.Parameters.Add(new SqlParameter("@Id", searchId));
            cmd.Parameters.Add(new SqlParameter("@Name", txtSearchProducts.Text));

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();

            SqlDataAdapter dap = new SqlDataAdapter(cmd);

            dap.Fill(dt);
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.DataSource = dt;
            dgvProducts.Refresh();
            cmd.Cancel();
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow dgvRow = dgvUsers.Rows[e.RowIndex];
                lbIdUser.Text = dgvRow.Cells["Id"].Value.ToString();
                lbNameUser.Text = dgvRow.Cells["Names"].Value.ToString();
                lbPhoneUser.Text = dgvRow.Cells["Phone"].Value.ToString();
                lbEmailUser.Text = dgvRow.Cells["Email"].Value.ToString();
                lbAddressUser.Text = dgvRow.Cells["Address"].Value.ToString();
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow dgvRow = dgvProducts.Rows[e.RowIndex];
                DialogResult dialogResult = MessageBox.Show("Thêm sản phẩm '"+ dgvRow.Cells["Namep"].Value.ToString() + "' vào giỏ hàng?", "Thông báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (Int32.Parse(dgvRow.Cells["Quantity"].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("Số lượng trong kho không đủ");
                        return;
                    }
                    Product product = new Product();
                    product.Id = dgvRow.Cells["Idp"].Value.ToString();
                    product.Name = dgvRow.Cells["Namep"].Value.ToString();
                    product.Brand = dgvRow.Cells["Brand"].Value.ToString();
                    product.Quantity = 1.ToString();
                    product.SalePrice = Math.Round((double)(Double.Parse(dgvRow.Cells["Sale_price"].Value.ToString()))).ToString();
                    product.Total = Math.Round((double)(Double.Parse(dgvRow.Cells["Sale_price"].Value.ToString()))).ToString();

                    if (dgvCarts.RowCount > 0)
                    {
                        for (int i = 0; i < dgvCarts.RowCount; i++)
                        {
                            DataGridViewRow dgvRowCarts = dgvCarts.Rows[i];
                            if (Int32.Parse(dgvRowCarts.Cells["idDataGridViewTextBoxColumn"].Value.ToString()) == Int32.Parse(dgvRow.Cells["Idp"].Value.ToString()))
                            {
                                if (Int32.Parse(dgvRowCarts.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString()) >= Int32.Parse(dgvRow.Cells["Quantity"].Value.ToString()))
                                {
                                    MessageBox.Show("Số lượng trong kho không đủ");
                                    return;
                                }
                                dgvRowCarts.Cells["quantityDataGridViewTextBoxColumn"].Value = Int32.Parse(dgvRowCarts.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString()) + 1;
                                dgvRowCarts.Cells["totalDataGridViewTextBoxColumn"].Value = Int32.Parse(dgvRowCarts.Cells["salePriceDataGridViewTextBoxColumn"].Value.ToString()) * Int32.Parse(dgvRowCarts.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString());
                                SumMoney();
                                return;
                            }
                        }
                    } 
                    productBindingSource.Add(product);

                    dgvCarts.DataSource = productBindingSource;
                    dgvCarts.Refresh();
                    SumMoney();

                }
            }
        }

        private void dgvCarts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCarts.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Bạn có muốn xóa sản phẩm này khỏi giỏ hàng không?", "Thông báo", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    productBindingSource.RemoveCurrent();
                    SumMoney();
                }
            }
        }

        string quantityHistory;

        private void dgvCarts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow dgvRow = dgvCarts.Rows[e.RowIndex];
                for (int i = 0; i < productBindingSource.Count; i++)
                {
                    DataGridViewRow dgvRowProduct = dgvProducts.Rows[i];
                    if (Int32.Parse(dgvRow.Cells["idDataGridViewTextBoxColumn"].Value.ToString()) == Int32.Parse(dgvRowProduct.Cells["Idp"].Value.ToString()))
                    {
                        dgvRow.Cells["totalDataGridViewTextBoxColumn"].Value = Int32.Parse(dgvRow.Cells["salePriceDataGridViewTextBoxColumn"].Value.ToString()) * Int32.Parse(dgvRow.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString());
                        if (Int32.Parse(dgvRow.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString()) > Int32.Parse(dgvRowProduct.Cells["Quantity"].Value.ToString()))
                        {
                            MessageBox.Show("Số lượng trong kho không đủ");
                            dgvRow.Cells["quantityDataGridViewTextBoxColumn"].Value = quantityHistory;
                        }
                    }
                }
                SumMoney();
            }
        }

        private void dgvCarts_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow dgvRow = dgvCarts.Rows[e.RowIndex];
                quantityHistory = dgvRow.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString();
            }
        }

        private void SumMoney()
        {
            if(dgvCarts.RowCount > 0)
            {
                Int64 sum = 0;
                for (int i = 0; i < dgvCarts.RowCount; i++)
                {
                    DataGridViewRow dgvRowCarts = dgvCarts.Rows[i];
                    int quantity = int.Parse(dgvRowCarts.Cells["salePriceDataGridViewTextBoxColumn"].Value.ToString()) * int.Parse(dgvRowCarts.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString());
                    
                    sum += quantity;

                }
                lbSumMoney.Text = Functions.FormatMoney(sum.ToString());
                totalMoney = sum.ToString();
            }
            else
            {
                lbSumMoney.Text = "...";
                totalMoney = "";
            }
        }
        
        private void ResetOrders()
        {
            lbIdUser.Text = "...";
            lbNameUser.Text = "...";
            lbEmailUser.Text = "...";
            lbPhoneUser.Text = "...";
            lbAddressUser.Text = "...";
            lbSumMoney.Text = "...";
            totalMoney = "";
            dgvCarts.Rows.Clear();
            dgvCarts.Refresh();
        }

        public static bool isClosePayment = false;

        private void btnCreateCart_Click(object sender, EventArgs e)
        {
            if (lbIdUser.Text.Equals("..."))
            {
                MessageBox.Show("Hãy chọn khách hàng trước!");
                return;
            }
            if (dgvCarts.RowCount <= 0)
            {
                MessageBox.Show("Hãy chọn sản phẩm vào giỏ hàng trước!");
                return;
            }

            frmPayment frm = new frmPayment();
            frm.Payment(Int64.Parse(totalMoney));
            frm.ShowDialog();

            if (isClosePayment) 
            {
                return;
            }

            string idOrder = "ID" + DateTime.Now.ToString("yyMMddHHmmssff");

            SqlCommand cmd = Functions.RunProcedure("CreateOrders");

            cmd.Parameters.Add(new SqlParameter("@Id", idOrder));
            cmd.Parameters.Add(new SqlParameter("@Customer_id", lbIdUser.Text));
            cmd.Parameters.Add(new SqlParameter("@Money", totalMoney));
            cmd.Parameters.Add(new SqlParameter("@Personnel_id", userID));
            cmd.Parameters.Add(new SqlParameter("@Day", DateTime.Now.Date.ToString("dd")));
            cmd.Parameters.Add(new SqlParameter("@Month", DateTime.Now.Date.ToString("MM")));
            cmd.Parameters.Add(new SqlParameter("@Year", DateTime.Now.Date.ToString("yyyy")));
            cmd.Parameters.Add(new SqlParameter("@Date_created", DateTime.Now.Date.ToString("MM/dd/yyyy")));

            cmd.ExecuteNonQuery();
            cmd.Cancel();

            foreach (DataGridViewRow row in dgvCarts.Rows)
            {
                SqlCommand cmdo = Functions.RunProcedure("CreateOrderProducts");

                cmdo.Parameters.Add(new SqlParameter("@Id_product", row.Cells["idDataGridViewTextBoxColumn"].Value));
                cmdo.Parameters.Add(new SqlParameter("@Id_order", idOrder));
                cmdo.Parameters.Add(new SqlParameter("@Quantity", row.Cells["quantityDataGridViewTextBoxColumn"].Value));
                cmdo.Parameters.Add(new SqlParameter("@Total", row.Cells["totalDataGridViewTextBoxColumn"].Value));
                cmdo.Parameters.Add(new SqlParameter("@Date_created", DateTime.Now.Date.ToString("MM/dd/yyyy")));

                cmdo.ExecuteNonQuery();
                cmdo.Cancel();

                for (int i = 0; i < quantiyEdit.Count; i++)
                {
                    if (quantiyEdit[i].Id.Equals(row.Cells["idDataGridViewTextBoxColumn"].Value))
                    {
                        int qt = int.Parse(quantiyEdit[i].Quantity) - int.Parse(row.Cells["quantityDataGridViewTextBoxColumn"].Value.ToString());
                        SqlCommand cmdp = Functions.RunProcedure("UpdateQuantityProducts");

                        cmdp.Parameters.Add(new SqlParameter("@Id", row.Cells["idDataGridViewTextBoxColumn"].Value));
                        cmdp.Parameters.Add(new SqlParameter("@Quantity", qt));
                        cmdp.Parameters.Add(new SqlParameter("@Date_edit", DateTime.Now.Date.ToString("MM/dd/yyyy")));

                        cmdp.ExecuteNonQuery();
                        cmdp.Cancel();
                    }
                }
            }
            LoadDataGridView("GetProducts", dgvProducts);
            frmDetailOrder frmDO = new frmDetailOrder();
            frmDO.DataFormChill(idOrder, userID, lbIdUser.Text, totalMoney);
            frmDO.ShowDialog();
            ResetOrders();
        }
    }
}
