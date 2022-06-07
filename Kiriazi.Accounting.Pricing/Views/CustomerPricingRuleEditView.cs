using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.ViewModels;
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
    public partial class CustomerPricingRuleEditView : Form
    {
        private readonly CustomerController _controller;
        private BindingList<CustomerPricingRule> _rules = new BindingList<CustomerPricingRule>();
        private CustomerPricingRulesEditViewModel _model;
        public CustomerPricingRuleEditView(
            CustomerController controller,
            CustomerPricingRulesEditViewModel model)
        {
            _controller = controller;
            _model = model;
            foreach(var rule in _model.CustomerRules)
            {
                _rules.Add(rule);
            }
            InitializeComponent();
            Initialize();
            
        }
        private void Initialize()
        {
            Text += _model.Customer.Name;
            //
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.RuleType),
                    DataPropertyName = nameof(CustomerPricingRule.RuleType),
                    HeaderText = "Rule Type",
                    DataSource = _model.PricingRuleTypes,
                    ReadOnly = false,
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.ItemCode),
                    DataPropertyName = nameof(CustomerPricingRule.ItemCode),
                    HeaderText = "Item",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.ItemType),
                    DataPropertyName = nameof(CustomerPricingRule.ItemType),
                    HeaderText = "Item Type",
                    ReadOnly = true,
                    Visible = true,
                    DataSource = _model.ItemTypes,
                    DisplayMember = nameof(ItemType.Name),
                    ValueMember = nameof(ItemType.Self)
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.Group),
                    DataPropertyName = nameof(CustomerPricingRule.Group),
                    DataSource = _model.Groups,
                    DisplayMember = nameof(Group.Name),
                    ValueMember = nameof(Group.Self),
                    HeaderText = "Group",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.Company),
                    DataPropertyName = nameof(CustomerPricingRule.Company),
                    DataSource = _model.Companies,
                    DisplayMember = nameof(Company.Name),
                    ValueMember = nameof(Company.Self),
                    HeaderText = "Company",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.IncrementDecrement),
                    DataPropertyName = nameof(CustomerPricingRule.IncrementDecrement),
                    HeaderText = "Increment/Decrement",
                    DataSource = _model.IncrementDecrement,
                    ReadOnly = false,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.RuleAmountType),
                    DataPropertyName = nameof(CustomerPricingRule.RuleAmountType),
                    DataSource = _model.RuleAmountTypes,
                    HeaderText = "Fixed/Percentage",
                    Visible = true,
                    ReadOnly = false
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.Amount),
                    DataPropertyName = nameof(CustomerPricingRule.Amount),
                    HeaderText = "Amount",
                    Visible = true,
                    ReadOnly = false
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.AmountCurrency),
                    DataPropertyName = nameof(CustomerPricingRule.AmountCurrency),
                    DisplayMember = nameof(Currency.Code),
                    ValueMember = nameof(Currency.Self),
                    DataSource = _model.Currencies,
                    Visible = true,
                    ReadOnly = true,
                    HeaderText = "Fixed Amount Currency"
                });
            _rules = new BindingList<CustomerPricingRule>(_model.CustomerRules);
            dataGridView1.DataSource = _rules;
            dataGridView1.DefaultValuesNeeded += DataGridView1_DefaultValuesNeeded;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns[nameof(CustomerPricingRule.RuleType)].Index)
            {
                string ruleType = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                switch (ruleType)
                {
                    case CustomerPricingRuleTypes.AllItems:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        break;
                    case CustomerPricingRuleTypes.Company:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        break;
                    case CustomerPricingRuleTypes.Item:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        break;
                    case CustomerPricingRuleTypes.ItemGroup:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        break;
                    case CustomerPricingRuleTypes.ItemType:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = false;
                        break;
                }
            }
            else if (e.ColumnIndex == dataGridView1.Columns[nameof(CustomerPricingRule.RuleAmountType)].Index)
            {
                string amountType = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                switch (amountType)
                {
                    case RuleAmountTypes.Fixed:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.AmountCurrency)].ReadOnly = false;
                        break;
                    case RuleAmountTypes.Percentage:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.AmountCurrency)].ReadOnly = true;
                        break;
                }
            }
        }

        private void DataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[nameof(CustomerPricingRule.RuleType)].Value = CustomerPricingRuleTypes.AllItems;
            e.Row.Cells[nameof(CustomerPricingRule.ItemCode)].Value = null;
            e.Row.Cells[nameof(CustomerPricingRule.ItemType)].Value = _model.ItemTypes.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Group)].Value = _model.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Company)].Value = _model.Companies.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.IncrementDecrement)].Value = IncrementDecrementTypes.Increment;
            e.Row.Cells[nameof(CustomerPricingRule.RuleAmountType)].Value = RuleAmountTypes.Percentage;
            e.Row.Cells[nameof(CustomerPricingRule.Amount)].Value = 0M;
            e.Row.Cells[nameof(CustomerPricingRule.AmountCurrency)].Value = _model.Currencies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
        }
    }
}
