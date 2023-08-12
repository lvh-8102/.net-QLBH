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
using AForge.Video;
using AForge.Video.DirectShow;

namespace ShopApp
{
    public partial class frmUsers : Form
    {
        public frmUsers()
        {
            InitializeComponent();
        }

        FilterInfoCollection fic;
        VideoCaptureDevice vcd;

        private void frmUsers_Load(object sender, EventArgs e)
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
            fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo fi in fic)
                cbCamera.Items.Add(fi.Name);
            cbCamera.SelectedIndex = 0;
        }

        private void LoadDataGridView()
        {
            if (frmLogin.positionLogin.Equals("1"))
            {
                DataTable dt = Code.Functions.GetData("GetUsers");
                dgvUsers.AutoGenerateColumns = false;
                dgvUsers.DataSource = dt;
                dgvUsers.Refresh();
            }
            else
            {
                DataTable dt = Code.Functions.GetData("GetCustomerUsers");
                dgvUsers.AutoGenerateColumns = false;
                dgvUsers.DataSource = dt;
                dgvUsers.Refresh();
            }
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
            txtEmail.Enabled = status;
            dtBirth.Enabled = status;
            txtPhone.Enabled = status;
            cbbStatus.Enabled = status;
            cbbPosition.Enabled = status;
            txtPassword.Enabled = status;
            txtAddress.Enabled = status;
            btnAddImage.Enabled = status;
            btnCamera.Enabled = status;
            btnOpenCamera.Enabled = status;
            btnCloseCamera.Enabled = status;
        }

        private void ResetTextBox()
        {
            lbId.Text = "...";
            ptbAvatar.Image = null;
            txtName.Text = "";
            txtEmail.Text = "";
            dtBirth.Text = "";
            txtPhone.Text = "";
            cbbStatus.Text = "";
            txtAddress.Text = "";
            cbbPosition.Text = "";
            txtPassword.Text = "";
            txtAddress.Text = "";
        }

        private void ResetError()
        {
            lbNameError.Text = "";
            lbEmailError.Text = "";
            lbPhoneError.Text = "";
            lbImageError.Text = "";
            lbStatusError.Text = "";
            lbPositionError.Text = "";
            lbPasswordError.Text = "";
            lbAddressError.Text = "";
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                string positon = "";
                DataGridViewRow dgvRow = dgvUsers.Rows[e.RowIndex];

                if (dgvRow.Cells["Position"].Value.ToString().Equals("1"))
                {
                    positon = "Admin";
                }
                else if(dgvRow.Cells["Position"].Value.ToString().Equals("2"))
                {
                    positon = "Nhân viên";
                }
                else
                {
                    positon = "Khách hàng";
                }    

                lbId.Text = dgvRow.Cells["Id"].Value.ToString();
                txtName.Text = dgvRow.Cells["Names"].Value.ToString();
                ptbAvatar.Image = Code.Functions.ComvertBytesArrayToImage((byte[])dgvRow.Cells["Avatar"].Value);
                txtEmail.Text = dgvRow.Cells["Email"].Value.ToString();
                dtBirth.Text = dgvRow.Cells["Birth_date"].Value.ToString();
                txtPhone.Text = dgvRow.Cells["Phone"].Value.ToString();
                cbbStatus.Text = dgvRow.Cells["Status"].Value.ToString() == "1" ? "Mở" : "Khóa";
                if (frmLogin.positionLogin.Equals("1"))
                    cbbPosition.Text = positon;
                txtAddress.Text = dgvRow.Cells["Address"].Value.ToString();
                EnabledTextBox(true);
                btnAdd.Enabled = false;
                btnCreate.Enabled = false;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                if (frmLogin.positionLogin.Equals("2"))
                {
                    cbbPosition.Enabled = false;
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            int status = 1, position = 1;

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
                lbNameError.Text = "Tên người dùng không được để trống!";
                return;
            }
            else
            {
                lbNameError.Text = "";
            }

            if (txtEmail.Text.Trim().Equals(""))
            {
                lbEmailError.Text = "Email không được để trống!";
                return;
            }
            else
            {
                lbEmailError.Text = "";
            }

            if (!Code.Functions.IsValidEmail(txtEmail.Text.Trim()))
            {
                lbEmailError.Text = "Email không đúng định dạng!";
                return;
            }
            else
            {
                lbEmailError.Text = "";
            }

            if (Code.Functions.IsData("IsEmailUser", txtEmail.Text.Trim()))
            {
                lbEmailError.Text = "Email này đã tồn tại!";
                return;
            }
            else
            {
                lbEmailError.Text = "";
            }

            if (cbbStatus.SelectedIndex <= -1)
            {
                lbStatusError.Text = "Trạng thái không được để trống!";
                return;
            }
            else
            {
                lbStatusError.Text = "";
            }
            if (frmLogin.positionLogin.Equals("1"))
            {
                if (cbbPosition.SelectedIndex <= -1)
                {
                    lbPositionError.Text = "Chức vụ không được để trống!";
                    return;
                }
                else
                {
                    lbPositionError.Text = "";
                }
            }

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

            if (Code.Functions.IsData("IsPhoneUser", txtPhone.Text.Trim()))
            {
                lbPhoneError.Text = "Số điện thoại này đã tồn tại!";
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

            if (txtAddress.Text.Trim().Equals(""))
            {
                lbAddressError.Text = "Địa chỉ không được để trống!";
                return;
            }
            else
            {
                lbAddressError.Text = "";
            }

            if (cbbStatus.SelectedItem.Equals("Khóa"))
            {
                status = 2;
            }

            if (frmLogin.positionLogin.Equals("1"))
            {
                if (cbbPosition.SelectedItem.Equals("Nhân viên"))
                {
                    position = 2;
                }
                else if (cbbPosition.SelectedItem.Equals("Khách hàng"))
                {
                    position = 3;
                }
            }
            else
            {
                position = 3;
            }
            SqlCommand cmd = Code.Functions.RunProcedure("CreateUsers");

            cmd.Parameters.Add(new SqlParameter("@Name", txtName.Text));
            cmd.Parameters.Add(new SqlParameter("@Email", txtEmail.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@Avatar", Code.Functions.ComvertImageToBytes(ptbAvatar.Image)));
            cmd.Parameters.Add(new SqlParameter("@Birth_date", dtBirth.Text));
            cmd.Parameters.Add(new SqlParameter("@Status", status));
            cmd.Parameters.Add(new SqlParameter("@Position", position));
            cmd.Parameters.Add(new SqlParameter("@Phone", txtPhone.Text));
            cmd.Parameters.Add(new SqlParameter("@Address", txtAddress.Text));
            cmd.Parameters.Add(new SqlParameter("@Password", Code.Functions.MD5Hash(txtPassword.Text)));
            cmd.Parameters.Add(new SqlParameter("@Date_created", DateTime.Now.Date.ToString("MM/dd/yyyy")));
            cmd.Parameters.Add(new SqlParameter("@Date_edit", DateTime.Now.Date.ToString("MM/dd/yyyy")));

            cmd.ExecuteNonQuery();
            cmd.Cancel();
            LoadDataGridView();
            ResetTextBox();
            EnabledButton(false);
            EnabledTextBox(false);
            if (frmLogin.positionLogin.Equals("2"))
            {
                cbbPosition.Enabled = false;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int status = 1, position = 1;

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
                lbNameError.Text = "Tên người dùng không được để trống!";
                return;
            }
            else
            {
                lbNameError.Text = "";
            }

            if (txtEmail.Text.Trim().Equals(""))
            {
                lbEmailError.Text = "Email không được để trống!";
                return;
            }
            else
            {
                lbEmailError.Text = "";
            }

            if (!Code.Functions.IsValidEmail(txtEmail.Text.Trim()))
            {
                lbEmailError.Text = "Email không đúng định dạng!";
                return;
            }
            else
            {
                lbEmailError.Text = "";
            }

            if (cbbStatus.SelectedIndex <= -1)
            {
                lbStatusError.Text = "Trạng thái không được để trống!";
                return;
            }
            else
            {
                lbStatusError.Text = "";
            }

            if (frmLogin.positionLogin.Equals("1"))
            {
                if (cbbPosition.SelectedIndex <= -1)
                {
                    lbPositionError.Text = "Chức vụ không được để trống!";
                    return;
                }
                else
                {
                    lbPositionError.Text = "";
                }
            }

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

            if (txtAddress.Text.Trim().Equals(""))
            {
                lbAddressError.Text = "Địa chỉ không được để trống!";
                return;
            }
            else
            {
                lbAddressError.Text = "";
            }

            if (cbbStatus.SelectedItem.Equals("Khóa"))
            {
                status = 2;
            }

            if (frmLogin.positionLogin.Equals("1"))
            {
                if (cbbPosition.SelectedItem.Equals("Nhân viên"))
                {
                    position = 2;
                }
                else if (cbbPosition.SelectedItem.Equals("Khách hàng"))
                {
                    position = 3;
                }
            }
            else
            {
                position = 3;
            }
            SqlCommand cmd = Code.Functions.RunProcedure("UpdateUsers");

            cmd.Parameters.Add(new SqlParameter("@Id", lbId.Text));
            cmd.Parameters.Add(new SqlParameter("@Name", txtName.Text));
            cmd.Parameters.Add(new SqlParameter("@Email", txtEmail.Text));
            cmd.Parameters.Add(new SqlParameter("@Avatar", Code.Functions.ComvertImageToBytes(ptbAvatar.Image)));
            cmd.Parameters.Add(new SqlParameter("@Birth_date", dtBirth.Text));
            cmd.Parameters.Add(new SqlParameter("@Status", status));
            cmd.Parameters.Add(new SqlParameter("@Position", position));
            cmd.Parameters.Add(new SqlParameter("@Phone", txtPhone.Text));
            cmd.Parameters.Add(new SqlParameter("@Address", txtAddress.Text));
            cmd.Parameters.Add(new SqlParameter("@Password", Code.Functions.MD5Hash(txtPassword.Text)));
            cmd.Parameters.Add(new SqlParameter("@Date_edit", DateTime.Now.Date.ToString("MM/dd/yyyy")));

            cmd.ExecuteNonQuery();
            cmd.Cancel();
            LoadDataGridView();
            ResetTextBox();
            EnabledButton(false);
            EnabledTextBox(false);
            if (frmLogin.positionLogin.Equals("2"))
            {
                cbbPosition.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng '" + txtName.Text + "' không?", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SqlCommand cmd = Code.Functions.RunProcedure("DeleteUsers");

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
            if (frmLogin.positionLogin.Equals("2"))
            {
                cbbPosition.Enabled = false;
            }
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
            string searchId = "", searchEmail = "", searchPhone = "";

            SqlCommand cmd = null;
            if (Code.Functions.IsPhoneNumber(txtSearch.Text.Trim()))
            {
                searchPhone = txtSearch.Text;
            }
            else if (Code.Functions.IsValidEmail(txtSearch.Text.Trim()))
            {
                searchEmail = txtSearch.Text;
            }
            else if (int.TryParse(txtSearch.Text, out int i))
            {
                searchId = txtSearch.Text;
            }

            if (frmLogin.positionLogin.Equals("1"))
            {
                cmd = Code.Functions.RunProcedure("SearchUsers");
                cmd.Parameters.Add(new SqlParameter("@Id", searchId));
                cmd.Parameters.Add(new SqlParameter("@Name", txtSearch.Text));
                cmd.Parameters.Add(new SqlParameter("@Email", searchEmail));
                cmd.Parameters.Add(new SqlParameter("@Phone", searchPhone));

                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd = Code.Functions.RunProcedure("SearchCustommerUsers");
                cmd.Parameters.Add(new SqlParameter("@Id", searchId));
                cmd.Parameters.Add(new SqlParameter("@Name", txtSearch.Text));
                cmd.Parameters.Add(new SqlParameter("@Email", searchEmail));
                cmd.Parameters.Add(new SqlParameter("@Phone", searchPhone));

                cmd.ExecuteNonQuery();
            }
            DataTable dt = new DataTable();

            SqlDataAdapter dap = new SqlDataAdapter(cmd);

            dap.Fill(dt);
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.DataSource = dt;
            dgvUsers.Refresh();
            cmd.Cancel();
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if (ofdImage.ShowDialog() == DialogResult.OK)
            {
                ptbAvatar.Image = Image.FromFile(ofdImage.FileName);

            }
        }

        private void Vcd_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            ptbAvatar.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            if (vcd.IsRunning)
                vcd.Stop();
        }

        private void btnOpenCamera_Click(object sender, EventArgs e)
        {
            vcd = new VideoCaptureDevice(fic[cbCamera.SelectedIndex].MonikerString);
            vcd.NewFrame += Vcd_NewFrame;
            vcd.Start();
        }

        private void btnCloseCamera_Click(object sender, EventArgs e)
        {
            if (vcd.IsRunning)
                vcd.Stop();
            ptbAvatar.Image = null;
        }
    }
}
