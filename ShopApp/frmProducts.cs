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
    public partial class frmProducts : Form
    {
        public frmProducts()
        {
            InitializeComponent();
        }

        List<ComboboxItem> listCategories = new List<ComboboxItem>();

        private void frmProducts_Load(object sender, EventArgs e)
        {
            btnSearch.BackColor = frmHome.color;
            btnCreate.BackColor = frmHome.color;
            btnEdit.BackColor = frmHome.color;
            btnDelete.BackColor = frmHome.color;
            btnExit.BackColor = frmHome.color;
            ResetTextBox();
            ResetError();
            EnabledButton(false);
            EnabledTextBox(false);
            LoadDataGridView();
            SqlCommand cmd = Functions.RunProcedure("GetCategories");
            cmd.ExecuteNonQuery();
            SqlDataReader data = cmd.ExecuteReader();

            while (data.Read())
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = data["Name"].ToString();
                item.Value = data["Id"].ToString();

                listCategories.Add(item);

                cbCategory.Items.Add(item);
            }
            data.Close();
            cmd.Cancel();
        }

        private void LoadDataGridView()
        {
            DataTable dt = Functions.GetData("GetProducts");
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.DataSource = dt;
            dgvProducts.Refresh();
        }

        private void EnabledButton(bool status)
        {
            btnCreate.Enabled = status;
            btnEdit.Enabled = status;
            btnDelete.Enabled = status;
        }

        private void EnabledTextBox(bool status)
        {
            txtName.Enabled = status;
            txtBrand.Enabled = status;
            txtQuantity.Enabled = status;
            txtOriginPrice.Enabled = status;
            txtSalePrice.Enabled = status;
            cbCategory.Enabled = status;
            cbStatus.Enabled = status;
            txtContent.Enabled = status;
            btnAddImage.Enabled = status;
        }

        private void ResetTextBox()
        {
            lbId.Text = "...";
            ptbAvatar.Image = null;
            txtName.Text = "";
            txtBrand.Text = "";
            txtQuantity.Text = "";
            txtOriginPrice.Text = "";
            txtSalePrice.Text = "";
            cbCategory.Text = "";
            cbStatus.Text = "";
            txtContent.Text = "";
        }

        private void ResetError()
        {
            lbNameError.Text = "";
            lbBrandError.Text = "";
            lbQuantityError.Text = "";
            lbCategoryError.Text = "";
            lbImageError.Text = "";
            lbOriginPriceError.Text = "";
            lbSalePriceError.Text = "";
            lbStatusError.Text = "";
            lbContentError.Text = "";
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow dgvRow = dgvProducts.Rows[e.RowIndex];
                lbId.Text = dgvRow.Cells["Id"].Value.ToString();
                txtName.Text = dgvRow.Cells["Names"].Value.ToString();
                ptbAvatar.Image = Code.Functions.ComvertBytesArrayToImage((byte[])dgvRow.Cells["Avatar"].Value);
                txtBrand.Text = dgvRow.Cells["Brand"].Value.ToString();
                txtQuantity.Text = dgvRow.Cells["Quantity"].Value.ToString();
                txtOriginPrice.Text = Math.Round((double)(Double.Parse(dgvRow.Cells["Origin_price"].Value.ToString()))).ToString();
                txtSalePrice.Text = Math.Round((double)(Double.Parse(dgvRow.Cells["Sale_price"].Value.ToString()))).ToString();
                cbStatus.Text = dgvRow.Cells["Status"].Value.ToString() == "1" ? "Còn hàng" : "Hết hàng";
                txtContent.Text = dgvRow.Cells["Content"].Value.ToString();
                for (int i = 0; i < listCategories.Count(); i++)
                {
                    if (listCategories[i].Value.ToString() == dgvRow.Cells["Category_id"].Value.ToString())
                    {
                        cbCategory.SelectedIndex = i;
                    }
                }
                EnabledTextBox(true);
                btnAdd.Enabled = false;
                btnCreate.Enabled = false;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            int status = 1;

            if (ptbAvatar.Image == null)
            {
                lbImageError.Text = "Ảnh đại diện không được để trống!";
                return;
            }
            else
            {
                lbImageError.Text = "";
            }

            if (txtName.Text.Trim().Equals(""))
            {
                lbNameError.Text = "Tên sản phẩm không được để trống!";
                return;
            }
            else
            {
                lbNameError.Text = "";
            }

            if (txtBrand.Text.Trim().Equals(""))
            {
                lbBrandError.Text = "Hãng không được để trống!";
                return;
            }
            else
            {
                lbBrandError.Text = "";
            }

            if (txtQuantity.Text.Trim().Equals(""))
            {
                lbQuantityError.Text = "Số lượng không được để trống!";
                return;
            }
            else
            {
                lbQuantityError.Text = "";
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int i))
            {
                lbQuantityError.Text = "Số lượng không được chứa kí tự!";
                return;
            }
            else
            {
                lbQuantityError.Text = "";
            }

            if (txtOriginPrice.Text.Trim().Equals(""))
            {
                lbOriginPriceError.Text = "Giá gốc không được để trống!";
                return;
            }
            else
            {
                lbOriginPriceError.Text = "";
            }

            if (!int.TryParse(txtOriginPrice.Text.Trim(), out int j))
            {
                lbOriginPriceError.Text = "Giá gốc không được chứa kí tự!";
                return;
            }
            else
            {
                lbOriginPriceError.Text = "";
            }

            if (txtSalePrice.Text.Trim().Equals(""))
            {
                lbSalePriceError.Text = "Giá bán không được để trống!";
                return;
            }
            else
            {
                lbSalePriceError.Text = "";
            }

            if (!int.TryParse(txtSalePrice.Text.Trim(), out int k))
            {
                lbSalePriceError.Text = "Giá bán không được chứa kí tự!";
                return;
            }
            else
            {
                lbSalePriceError.Text = "";
            }

            if (cbCategory.SelectedIndex <= -1)
            {
                lbCategoryError.Text = "Danh mục không được để trống!";
                return;
            }
            else
            {
                lbCategoryError.Text = "";
            }

            if (cbStatus.SelectedIndex <= -1)
            {
                lbStatusError.Text = "Trạng thái không được để trống!";
                return;
            }
            else
            {
                lbStatusError.Text = "";
            }

            if (txtContent.Text.Trim().Equals(""))
            {
                lbContentError.Text = "Mô tả không được để trống!";
                return;
            }
            else
            {
                lbContentError.Text = "";
            }

            if (cbStatus.SelectedItem.Equals("Hết hàng"))
            {
                status = 2;
            }
            int origin = Int32.Parse(txtOriginPrice.Text);
            int salep = Int32.Parse(txtSalePrice.Text);
            float discount1 = origin - salep;
            float discount2 = discount1 / salep;
            double discount = Math.Round((double)(discount2 * 100));

            SqlCommand cmd = Functions.RunProcedure("CreateProducts");

            cmd.Parameters.Add(new SqlParameter("@Name", txtName.Text));
            cmd.Parameters.Add(new SqlParameter("@Brand", txtBrand.Text));
            cmd.Parameters.Add(new SqlParameter("@Avatar", Functions.ComvertImageToBytes(ptbAvatar.Image)));
            cmd.Parameters.Add(new SqlParameter("@Quantity", Int32.Parse(txtQuantity.Text)));
            cmd.Parameters.Add(new SqlParameter("@Origin_price", txtOriginPrice.Text));
            cmd.Parameters.Add(new SqlParameter("@Sale_price", txtSalePrice.Text));
            cmd.Parameters.Add(new SqlParameter("@Discount_percent", discount));
            cmd.Parameters.Add(new SqlParameter("@Content", txtContent.Text));
            cmd.Parameters.Add(new SqlParameter("@User_id", frmLogin.idLogin));
            cmd.Parameters.Add(new SqlParameter("@Category_id", Int32.Parse((cbCategory.SelectedItem as ComboboxItem).Value.ToString())));
            cmd.Parameters.Add(new SqlParameter("@Status", status));
            cmd.Parameters.Add(new SqlParameter("@Date_created", DateTime.Now.Date.ToString("MM/dd/yyyy")));
            cmd.Parameters.Add(new SqlParameter("@Date_edit", DateTime.Now.Date.ToString("MM/dd/yyyy")));

            cmd.ExecuteNonQuery();
            cmd.Cancel();
            LoadDataGridView();
            ResetTextBox();
            EnabledButton(false);
            EnabledTextBox(false);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int status = 1;

            if (ptbAvatar.Image == null)
            {
                lbImageError.Text = "Ảnh đại diện không được để trống!";
                return;
            }
            else
            {
                lbImageError.Text = "";
            }

            if (txtName.Text.Trim().Equals(""))
            {
                lbNameError.Text = "Tên sản phẩm không được để trống!";
                return;
            }
            else
            {
                lbNameError.Text = "";
            }

            if (txtBrand.Text.Trim().Equals(""))
            {
                lbBrandError.Text = "Hãng không được để trống!";
                return;
            }
            else
            {
                lbBrandError.Text = "";
            }

            if (txtQuantity.Text.Trim().Equals(""))
            {
                lbQuantityError.Text = "Số lượng không được để trống!";
                return;
            }
            else
            {
                lbQuantityError.Text = "";
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int i))
            {
                lbQuantityError.Text = "Số lượng không được chứa kí tự!";
                return;
            }
            else
            {
                lbQuantityError.Text = "";
            }

            if (txtOriginPrice.Text.Trim().Equals(""))
            {
                lbOriginPriceError.Text = "Giá gốc không được để trống!";
                return;
            }
            else
            {
                lbOriginPriceError.Text = "";
            }

            if (!int.TryParse(txtOriginPrice.Text.Trim(), out int j))
            {
                lbOriginPriceError.Text = "Giá gốc không được chứa kí tự!";
                return;
            }
            else
            {
                lbOriginPriceError.Text = "";
            }

            if (txtSalePrice.Text.Trim().Equals(""))
            {
                lbSalePriceError.Text = "Giá bán không được để trống!";
                return;
            }
            else
            {
                lbSalePriceError.Text = "";
            }

            if (!int.TryParse(txtSalePrice.Text.Trim(), out int k))
            {
                lbSalePriceError.Text = "Giá bán không được chứa kí tự!";
                return;
            }
            else
            {
                lbSalePriceError.Text = "";
            }

            if (cbCategory.SelectedIndex <= -1)
            {
                lbCategoryError.Text = "Danh mục không được để trống!";
                return;
            }
            else
            {
                lbCategoryError.Text = "";
            }

            if (cbStatus.SelectedIndex <= -1)
            {
                lbStatusError.Text = "Trạng thái không được để trống!";
                return;
            }
            else
            {
                lbStatusError.Text = "";
            }

            if (txtContent.Text.Trim().Equals(""))
            {
                lbContentError.Text = "Mô tả không được để trống!";
                return;
            }
            else
            {
                lbContentError.Text = "";
            }

            if (cbStatus.SelectedItem.Equals("Hết hàng"))
            {
                status = 2;
            }
            int origin = Int32.Parse(txtOriginPrice.Text);
            int salep = Int32.Parse(txtSalePrice.Text);
            float discount1 = origin - salep;
            float discount2 = discount1 / salep;
            double discount = Math.Round((double)(discount2 * 100));

            SqlCommand cmd = Functions.RunProcedure("UpdateProducts");

            cmd.Parameters.Add(new SqlParameter("@Id", lbId.Text));
            cmd.Parameters.Add(new SqlParameter("@Name", txtName.Text));
            cmd.Parameters.Add(new SqlParameter("@Brand", txtBrand.Text));
            cmd.Parameters.Add(new SqlParameter("@Avatar", Functions.ComvertImageToBytes(ptbAvatar.Image)));
            cmd.Parameters.Add(new SqlParameter("@Quantity", Int32.Parse(txtQuantity.Text)));
            cmd.Parameters.Add(new SqlParameter("@Origin_price", txtOriginPrice.Text));
            cmd.Parameters.Add(new SqlParameter("@Sale_price", txtSalePrice.Text));
            cmd.Parameters.Add(new SqlParameter("@Discount_percent", discount));
            cmd.Parameters.Add(new SqlParameter("@Content", txtContent.Text));
            cmd.Parameters.Add(new SqlParameter("@User_id", frmLogin.idLogin));
            cmd.Parameters.Add(new SqlParameter("@Category_id", Int32.Parse((cbCategory.SelectedItem as ComboboxItem).Value.ToString())));
            cmd.Parameters.Add(new SqlParameter("@Status", status));
            cmd.Parameters.Add(new SqlParameter("@Date_edit", DateTime.Now.Date.ToString("MM/dd/yyyy")));

            cmd.ExecuteNonQuery();
            cmd.Cancel();
            LoadDataGridView();
            ResetTextBox();
            EnabledButton(false);
            EnabledTextBox(false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm '" + txtName.Text + "' không?", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SqlCommand cmd = Code.Functions.RunProcedure("DeleteProducts");

                cmd.Parameters.Add(new SqlParameter("@Id", lbId.Text));

                cmd.ExecuteNonQuery();
                cmd.Cancel();
                LoadDataGridView();
                ResetTextBox();
                EnabledButton(false);
                EnabledTextBox(false);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EnabledTextBox(true);
            btnCreate.Enabled = true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            ResetTextBox();
            ResetError();
            EnabledButton(false);
            EnabledTextBox(false);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchId = "";

            SqlCommand cmd = Code.Functions.RunProcedure("SearchProducts");
            if (int.TryParse(txtSearch.Text, out int i))
            {
                searchId = txtSearch.Text;
            }
            cmd.Parameters.Add(new SqlParameter("@Id", searchId));
            cmd.Parameters.Add(new SqlParameter("@Name", txtSearch.Text));

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();

            SqlDataAdapter dap = new SqlDataAdapter(cmd);

            dap.Fill(dt);
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.DataSource = dt;
            dgvProducts.Refresh();
            cmd.Cancel();
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if (ofdImage.ShowDialog() == DialogResult.OK)
            {
                ptbAvatar.Image = Image.FromFile(ofdImage.FileName);

            }
        }
    }
}
