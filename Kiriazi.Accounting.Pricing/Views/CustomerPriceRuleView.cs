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
    public partial class CustomerPriceRuleView : Form
    {
        private readonly BindingList<ViewModels.CustomerPricingRuleViewModel> _rules;
        public CustomerPriceRuleView(IList<ViewModels.CustomerPricingRuleViewModel> rules)
        {
            _rules = new BindingList<ViewModels.CustomerPricingRuleViewModel>(rules);
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.AccountingPeriodName),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.AccountingPeriodName),
                    ReadOnly = true,
                    HeaderText = "Accounting Period"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.CustomerName),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.CustomerName),
                    ReadOnly = true,
                    HeaderText = "Customer"
                }, 
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.RuleType),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.RuleType),
                    ReadOnly = true,
                    HeaderText = "Rule Type"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.ItemTypeName),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.ItemTypeName),
                    ReadOnly = true,
                    HeaderText = "Item Type"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.GroupName),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.GroupName),
                    ReadOnly = true,
                    HeaderText = "Group"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.CompanyName),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.CompanyName),
                    ReadOnly = true,
                    HeaderText = "Company"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.ItemCode),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.ItemCode),
                    ReadOnly = true,
                    HeaderText = "Group"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.Amount),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.Amount),
                    ReadOnly = true,
                    HeaderText = "Amount"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.AmountType),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.AmountType),
                    ReadOnly = true,
                    HeaderText = "Amount Type"
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(ViewModels.CustomerPricingRuleViewModel.IncrementDecrement),
                    Name = nameof(ViewModels.CustomerPricingRuleViewModel.IncrementDecrement),
                    ReadOnly = true,
                    HeaderText = "Increment / Decrement"
                });
            dataGridView1.DataSource = _rules;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
