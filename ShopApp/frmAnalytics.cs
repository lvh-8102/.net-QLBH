using LiveCharts;
using LiveCharts.Wpf;
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
    public partial class frmAnalytics : Form
    {
        public frmAnalytics()
        {
            InitializeComponent();
        }

        private void frmAnalytics_Load(object sender, EventArgs e)
        {
            btnSearch.BackColor = frmHome.color;
            btnFilter.BackColor = frmHome.color;
            lbYearError.Text = "";
            lbMonthError.Text = "";
            cbMonth.Enabled = false;
            LoadDataGridView();
            SqlCommand cmdy = Functions.RunProcedure("GetOrdersYear");
            cmdy.ExecuteNonQuery();
            SqlDataReader datay = cmdy.ExecuteReader();

            while (datay.Read())
            {
                cbYear.Items.Add(datay[0].ToString().Trim());
            }
            datay.Close();
            cmdy.Cancel();

            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Ngày",
                Labels = new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13",
                    "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27",
                    "28", "29", "30", "31" }
            });
            cartesianChart1.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Doanh thu",
                LabelFormatter = value => value.ToString(".00 VND")
            });

            cartesianChart1.LegendLocation = LiveCharts.LegendLocation.Right;
        }

        private void LoadDataGridView()
        {
            DataTable dt = Functions.GetData("GetOrders");
            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.DataSource = dt;
            dgvOrders.Refresh();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (cbYear.SelectedIndex <= -1)
            {
                lbYearError.Text = "Năm không được để trống!";
                return;
            }
            else
            {
                lbYearError.Text = "";
            }

            if (cbMonth.SelectedIndex <= -1)
            {
                lbMonthError.Text = "Tháng không được để trống!";
                return;
            }
            else
            {
                lbMonthError.Text = "";
            }
            cartesianChart1.Series.Clear();
            SqlCommand cmd = Functions.RunProcedure("GetOrdersDay");
            cmd.Parameters.Add(new SqlParameter("@Month", cbMonth.SelectedItem.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Year", cbYear.SelectedItem.ToString()));
            cmd.ExecuteNonQuery();
            SqlDataReader data = cmd.ExecuteReader();
            
            SeriesCollection series = new SeriesCollection();
            List<double> values = new List<double>();
            List<Analytic> day = new List<Analytic>();

            while (data.Read())
            {
                Analytic ana = new Analytic();
                ana.Day = int.Parse(data[0].ToString());
                ana.Money = double.Parse(data[1].ToString());
                day.Add(ana);
            }

            for (int i = 1; i <= 31; i++)
            {
                double value = 0;
                bool isAddItem = true;
                foreach (Analytic item in day)
                {
                    if (item.Day == i)
                    {
                        isAddItem = false;
                        values.Add(item.Money);
                    }
                }
                if (isAddItem)
                {
                    values.Add(value);
                }
            }
            series.Add(new LineSeries() { Title = cbMonth.SelectedItem.ToString(), Values = new ChartValues<double>(values) });
            cartesianChart1.Series = series;
            data.Close();
            cmd.Cancel();
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbMonth.Items.Clear();
            SqlCommand cmdm = Functions.RunProcedure("GetOrdersMonth");
            cmdm.Parameters.Add(new SqlParameter("@Year", cbYear.SelectedItem.ToString()));
            cmdm.ExecuteNonQuery();
            SqlDataReader datam = cmdm.ExecuteReader();

            while (datam.Read())
            {
                cbMonth.Items.Add(datam[0].ToString());
            }
            cbMonth.Enabled = true;
            datam.Close();
            cmdm.Cancel();
        }

        private void dgvOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvOrders.Columns[e.ColumnIndex].Name == "Detail")
            {
                DataGridViewRow dgvRow = dgvOrders.Rows[e.RowIndex];
                frmDetailOrder frm = new frmDetailOrder();
                frm.DataFormChill(dgvRow.Cells["Id"].Value.ToString(), dgvRow.Cells["Personnel_id"].Value.ToString(), dgvRow.Cells["Customer_id"].Value.ToString(), dgvRow.Cells["Money"].Value.ToString());
                frm.ShowDialog();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = Code.Functions.RunProcedure("SearchOrders");
            cmd.Parameters.Add(new SqlParameter("@Id", txtSearch.Text));

            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();

            SqlDataAdapter dap = new SqlDataAdapter(cmd);

            dap.Fill(dt);
            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.DataSource = dt;
            dgvOrders.Refresh();
            cmd.Cancel();
        }
    }
}
