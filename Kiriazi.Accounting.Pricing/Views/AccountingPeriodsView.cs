using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiriazi.Accounting.Pricing.Views
{
    public partial class AccountingPeriodsView : Form
    {
        private readonly Controllers.AccountingPeriodController _controller;
        private BindingList<Models.AccountingPeriod> _accountingPeriods = new BindingList<Models.AccountingPeriod>();
        public AccountingPeriodsView(Controllers.AccountingPeriodController controller)
        {
            _controller = controller;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    Name = "Name",
                    DataPropertyName = "Name",
                    HeaderText = "Name",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "Description",
                    DataPropertyName = "Description",
                    HeaderText = "Description",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "FromDate",
                    DataPropertyName = "FromDate",
                    HeaderText = "From",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn() 
                { 
                    Name = "ToDate",
                    DataPropertyName = "ToDate",
                    HeaderText = "To",
                    ReadOnly = true
                },
                new DataGridViewButtonColumn()
                {
                    Name = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    HeaderText = "",
                    ReadOnly = true
                });
            dataGridView1.CellContentClick += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if(e.ColumnIndex == grid.Columns["Delete"].Index)
                {
                    DialogResult result = MessageBox.Show(
                        this, 
                        $"Are you sure you want to delete period {_accountingPeriods[e.RowIndex].Name}",
                        "Confirm Delete", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);
                    if(result == DialogResult.Yes)
                    {
                        string message = _controller.Delete(_accountingPeriods[e.RowIndex].Id);
                        if (message != string.Empty)
                        {
                            _ = MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                           
                        }
                        Search();
                    }
                }
            };
            dataGridView1.Columns["FromDate"].DefaultCellStyle.Format = "g";
            dataGridView1.Columns["ToDate"].DefaultCellStyle.Format = "g";
            Search();
            dataGridView1.DataSource = _accountingPeriods;

        }
        private void Search()
        {
            _accountingPeriods.Clear();
            var periods = _controller.Find();
            foreach(var period in periods)
            {
                _accountingPeriods.Add(period);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using(AccountingPeriodView accountingPeriodView = new AccountingPeriodView(_controller))
            {
                accountingPeriodView.ShowDialog(this);
                Search();
            }
        }
    }
}
