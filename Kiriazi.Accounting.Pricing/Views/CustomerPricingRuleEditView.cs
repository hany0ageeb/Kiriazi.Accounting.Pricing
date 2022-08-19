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
        private readonly AccountingPeriodController _controller;
        private BindingList<CustomerPricingRule> _rules = new BindingList<CustomerPricingRule>();
        private CustomerPricingRulesEditViewModel _model;
        private readonly AccountingPeriod _accountingPeriod;
        private bool _hasChanged = false;
        
        private readonly AutoCompleteStringCollection _autoCompleteSource = new AutoCompleteStringCollection();
        public CustomerPricingRuleEditView(
            AccountingPeriodController controller,
            CustomerPricingRulesEditViewModel model,
            AccountingPeriod accountingPeriod)
        {
            _controller = controller;
            _accountingPeriod = accountingPeriod;
            _model = model;
            foreach(var rule in _model.Rules)
            {
                _rules.Add(rule);
            }
            InitializeComponent();
            Initialize();
            
        }
        private void Initialize()
        {
            Text += _accountingPeriod.Name;
            //
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.Customer),
                    DataPropertyName = nameof(CustomerPricingRule.Customer),
                    HeaderText = "Customer",
                    DataSource = _model.Customers,
                    DisplayMember = nameof(Models.Customer.Name),
                    ValueMember = nameof(Models.Customer.Self),
                    Visible = true,
                    ReadOnly = false
                },
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
            _rules = new BindingList<CustomerPricingRule>(_model.Rules);
            dataGridView1.DataSource = _rules;
            dataGridView1.DefaultValuesNeeded += DataGridView1_DefaultValuesNeeded;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
            dataGridView1.RowValidating += DataGridView1_RowValidating;
            dataGridView1.RowValidated += DataGridView1_RowValidated;
            _autoCompleteSource.AddRange(_model.ItemsCodes.ToArray());

            _rules.ListChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
                if(e.ListChangedType == ListChangedType.ItemAdded)
                {
                    _rules[e.NewIndex].Id = Guid.Empty;
                }
            };
        }

        private void DataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = "";
        }

        private void DataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if(grid!=null && e.RowIndex >= 0 && e.RowIndex < _rules.Count)
            {
                switch (_rules[e.RowIndex].RuleType)
                {
                    case CustomerPricingRuleTypes.ItemInCompany:
                        if(_rules[e.RowIndex].Company==null || _rules[e.RowIndex].Company.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "select a company from the list.";
                        }
                        else if (string.IsNullOrEmpty(_rules[e.RowIndex].ItemCode))
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Item Code.";
                        }
                        break;
                    case CustomerPricingRuleTypes.AllItems:
                        if (!string.IsNullOrEmpty(_rules[e.RowIndex].ItemCode))
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "All Items Rule Type Cannot has Item Code!";
                        }
                        break;
                    case CustomerPricingRuleTypes.Company:
                        if(_rules[e.RowIndex].Company==null || _rules[e.RowIndex].Company.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "select a company from the list.";
                        }
                        break;
                    case CustomerPricingRuleTypes.Item:
                        if (string.IsNullOrEmpty(_rules[e.RowIndex].ItemCode))
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Item Code.";
                        }
                        else
                        {
                            if (!_autoCompleteSource.Contains(_rules[e.RowIndex].ItemCode))
                            {
                                e.Cancel = true;
                                System.Media.SystemSounds.Hand.Play();
                                dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Item Code.";
                            }
                            else
                            {

                            }
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemGroup:
                        if(_rules[e.RowIndex].Group==null || _rules[e.RowIndex].Group.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Select a Group from the list.";
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemType:
                        if(_rules[e.RowIndex].ItemType == null || _rules[e.RowIndex].ItemType.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Select an Item Type.";
                        }
                        break;
                }
                switch (_rules[e.RowIndex].RuleAmountType)
                {
                    case RuleAmountTypes.Fixed:
                        if(_rules[e.RowIndex].AmountCurrency == null || _rules[e.RowIndex].AmountCurrency.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Select Currency";
                        }
                        
                        break;
                    case RuleAmountTypes.Percentage:
                        break;
                }
                if(_rules[e.RowIndex].Amount < 0)
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Amount. Enter value greater than or equal to zero.";
                }
            }
        }
        
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int? index = dataGridView1.CurrentCell?.ColumnIndex;
            if(index != null && index == dataGridView1.Columns[nameof(CustomerPricingRule.ItemCode)].Index)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    textBox.AutoCompleteCustomSource = _autoCompleteSource;
                }
            }
            else
            {
                if (index != null)
                {
                    TextBox textBox = e.Control as TextBox;
                    if (textBox != null)
                    {
                        textBox.AutoCompleteMode = AutoCompleteMode.None;
                        textBox.AutoCompleteSource = AutoCompleteSource.None;
                        textBox.AutoCompleteCustomSource = null;
                    }
                }
            }
        }
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns[nameof(CustomerPricingRule.RuleType)].Index)
            {
                string ruleType = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                switch (ruleType)
                {
                    case CustomerPricingRuleTypes.ItemInCompany:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = false;
                        if (e.RowIndex >=0 && e.RowIndex < _rules.Count)
                        {
                            _rules[e.RowIndex].Group = _model.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].ItemType = _model.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.AllItems:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _rules.Count)
                        {
                            _rules[e.RowIndex].ItemCode = "";
                            _rules[e.RowIndex].Group = _model.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].Company = _model.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].ItemType = _model.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.Company:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _rules.Count)
                        {
                            _rules[e.RowIndex].ItemCode = "";
                            _rules[e.RowIndex].Group = _model.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].ItemType = _model.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].Company = _model.Companies.Where(c => c.Id != Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.Item:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _rules.Count)
                        {
                            _rules[e.RowIndex].Group = _model.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].Company = _model.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].ItemType = _model.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemGroup:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _rules.Count)
                        {
                            _rules[e.RowIndex].ItemCode = "";
                            _rules[e.RowIndex].Company = _model.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].ItemType = _model.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].Group = _model.Groups.Where(g => g.Id != Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemType:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = false;
                        if (e.RowIndex >= 0 && e.RowIndex < _rules.Count)
                        {
                            _rules[e.RowIndex].ItemCode = "";
                            _rules[e.RowIndex].Group = _model.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].Company = _model.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _rules[e.RowIndex].ItemType = _model.ItemTypes.Where(t => t.Id != Guid.Empty).FirstOrDefault();
                        }
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
                        if (e.RowIndex >= 0 && e.RowIndex < _rules.Count)
                        {
                            _rules[e.RowIndex].AmountCurrency = _model.Currencies.Where(c => c.Id != Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case RuleAmountTypes.Percentage:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.AmountCurrency)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _rules.Count)
                            _rules[e.RowIndex].AmountCurrency = _model.Currencies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        break;
                }
            }
        }
        private void DataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[nameof(CustomerPricingRule.RuleType)].Value = CustomerPricingRuleTypes.AllItems;
            e.Row.Cells[nameof(CustomerPricingRule.ItemCode)].Value = "";
            e.Row.Cells[nameof(CustomerPricingRule.ItemType)].Value = _model.ItemTypes.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Group)].Value = _model.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Company)].Value = _model.Companies.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.IncrementDecrement)].Value = IncrementDecrementTypes.Increment;
            e.Row.Cells[nameof(CustomerPricingRule.RuleAmountType)].Value = RuleAmountTypes.Percentage;
            e.Row.Cells[nameof(CustomerPricingRule.Amount)].Value = 0M;
            e.Row.Cells[nameof(CustomerPricingRule.AmountCurrency)].Value = _model.Currencies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Customer)].Value = _model.Customers.Where(c => c.Id == Guid.Empty).FirstOrDefault();
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
               // try
               // {
                    var modelState = _controller.SaveOrUpdatePricingRules(_model,_accountingPeriod);
                    if (modelState.HasErrors)
                    {
                        var errors = modelState.GetErrors("Customer");
                        if (errors.Count > 0)
                        {
                            _ = MessageBox.Show(
                                this, 
                                errors[0], 
                                "Error", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
                        }
                        for(int i = 0; i < modelState.InnerModelStatesCount; i++)
                        {
                            var temp = modelState.GetModelState(i);
                            if (temp.HasErrors)
                            {
                                if (i < dataGridView1.Rows.Count)
                                {
                                    dataGridView1.Rows[i].ErrorText = temp.GetErrors()[0];
                                }
                            }
                        }
                        return false;
                    }
                    else
                    {
                        _hasChanged = false;
                        System.Media.SystemSounds.Beep.Play();
                        return true;
                    }
               // }
                //catch(Exception ex)
                //{
                //    _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               //     return false;
                //}
            }
            return true;
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult reuslt = MessageBox.Show(this, "Do you want to save changes?", "Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if(reuslt == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChanged = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (reuslt == DialogResult.No)
                {
                    _hasChanged = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _hasChanged = false;
                Close();
            }
            else
            {

            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
            {
                Close();
            }
        }
        private void CustomerPricingRuleEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.MdiFormClosing))
            {
                e.Cancel = !CloseForm();
            }
        }

       
    }
}
