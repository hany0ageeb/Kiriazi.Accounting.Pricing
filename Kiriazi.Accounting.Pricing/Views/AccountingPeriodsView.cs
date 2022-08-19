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
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(Models.AccountingPeriod.Name),
                    DataPropertyName = nameof(Models.AccountingPeriod.Name),
                    HeaderText = "Name",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(Models.AccountingPeriod.Description),
                    DataPropertyName = nameof(Models.AccountingPeriod.Description),
                    HeaderText = "Description",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(Models.AccountingPeriod.FromDate),
                    DataPropertyName = nameof(Models.AccountingPeriod.FromDate),
                    HeaderText = "From",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn() 
                { 
                    Name = nameof(Models.AccountingPeriod.ToDate),
                    DataPropertyName = nameof(Models.AccountingPeriod.ToDate),
                    HeaderText = "To",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(Models.AccountingPeriod.State),
                    DataPropertyName = nameof(Models.AccountingPeriod.State),
                    HeaderText = "State",
                    ReadOnly = true
                },
                new DataGridViewButtonColumn()
                {
                    Name = "ChangeState",
                    Text = "Change State",
                    UseColumnTextForButtonValue = true,
                    HeaderText = "",
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
                else if(e.ColumnIndex == grid.Columns["ChangeState"].Index)
                {
                    if (e.RowIndex >= 0 && e.RowIndex < _accountingPeriods.Count)
                    {
                        _controller.ChangeState(_accountingPeriods[e.RowIndex].Id);
                        Search();
                    }
                }
            };
            dataGridView1.Columns[nameof(Models.AccountingPeriod.FromDate)].DefaultCellStyle.Format = "g";
            dataGridView1.Columns[nameof(Models.AccountingPeriod.ToDate)].DefaultCellStyle.Format = "g";
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

        private void btnPricingRules_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if (index != null && index >= 0 && index < _accountingPeriods.Count) 
            {
                var rules =_controller.EditCustomerPricingRules(_accountingPeriods[index.Value].Id);
                using(CustomerPricingRuleEditView pricingRuleEditView = new CustomerPricingRuleEditView(_controller, rules,_accountingPeriods[index.Value]))
                {
                    pricingRuleEditView.ShowDialog(this);
                }
            }
        }
    }
}
