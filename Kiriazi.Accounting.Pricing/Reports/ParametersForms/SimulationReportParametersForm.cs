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

namespace Kiriazi.Accounting.Pricing.Reports.ParametersForms
{
    public partial class SimulationReportParametersForm : Form
    {
        private SimulationReportParameterViewModel _model;
        private readonly Controllers.AccountingPeriodController _accountingPeriodController;
        private readonly Controllers.PriceListController _priceListController;
        private readonly Controllers.CurrencyExchangeRateController _currencyExchangeRateController;
        private BindingList<CustomerPricingRule> _periodRules = new BindingList<CustomerPricingRule>();
        
        private readonly AutoCompleteStringCollection _autoCompleteSource = new AutoCompleteStringCollection();
        public SimulationReportParametersForm(Controllers.AccountingPeriodController accountingPeriodController, 
            Controllers.PriceListController priceListController,
            Controllers.CurrencyExchangeRateController currencyExchangeRateController)
        {
            _accountingPeriodController = accountingPeriodController;
            _priceListController = priceListController;
            _currencyExchangeRateController = currencyExchangeRateController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            _model = _accountingPeriodController.GenerateSimulationReportData();
            //
            cboItems.DataBindings.Clear();
            cboItems.DataSource = _model.Items;
            cboItems.DisplayMember = nameof(_model.Item.Code);
            cboItems.ValueMember = nameof(_model.Item.Self);
            cboItems.DataBindings.Add(new Binding(nameof(cboItems.SelectedItem),_model,nameof(_model.Item)) { DataSourceUpdateMode = DataSourceUpdateMode.OnValidation });
            //
            cboPeriods.DataBindings.Clear();
            cboPeriods.DataSource = _model.AccountingPeriods;
            cboPeriods.DisplayMember = nameof(_model.AccountingPeriod.Name);
            cboPeriods.ValueMember = nameof(_model.AccountingPeriod.Self);
            cboPeriods.DataBindings.Add(new Binding(nameof(cboPeriods.SelectedItem),_model,nameof(_model.AccountingPeriod)) { DataSourceUpdateMode = DataSourceUpdateMode.OnValidation});
            //
            cboCurrency.DataSource = _model.Currencies;
            cboCurrency.DisplayMember = nameof(_model.ProposedCurrency.Code);
            cboCurrency.ValueMember = nameof(_model.ProposedCurrency.Self);
            cboCurrency.DataBindings.Clear();
            cboCurrency.DataBindings.Add(new Binding(nameof(cboCurrency.SelectedItem),_model,nameof(_model.ProposedCurrency))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            //
            cboCustomers.DataBindings.Clear();
            cboCustomers.DataSource = _model.Customers;
            cboCustomers.DisplayMember = nameof(_model.Customer.Name);
            cboCustomers.ValueMember = nameof(_model.Customer.Self);
            cboCustomers.DataBindings.Add(new Binding(nameof(cboCustomers.SelectedItem),_model,nameof(_model.Customer))
            {

            });
            //
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(_model.Company.Name);
            cboCompanies.ValueMember = nameof(_model.Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCustomers.SelectedItem), _model, nameof(_model.Company))
            {

            });
            //
            txtProposedUnitPrice.DataBindings.Clear();
            txtProposedUnitPrice.DataBindings.Add(new Binding(nameof(txtProposedUnitPrice.Text), _model, nameof(_model.ProposedUnitPrice))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            //
           
            //
            dataGridViewCurrentRules.AllowUserToAddRows = false;
            dataGridViewCurrentRules.AllowUserToDeleteRows = false;
            dataGridViewCurrentRules.ReadOnly = true;
            dataGridViewCurrentRules.AutoGenerateColumns = false;
            dataGridViewCurrentRules.Columns.Clear();
            dataGridViewCurrentRules.Columns.AddRange(
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.PricingRuleTypes,
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.ItemTypes,
                    DisplayMember = nameof(ItemType.Name),
                    ValueMember = nameof(ItemType.Self)
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.Group),
                    DataPropertyName = nameof(CustomerPricingRule.Group),
                    DataSource = _model.CustomerPricingRulesEditViewModel.Groups,
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.Companies,
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.IncrementDecrement,
                    ReadOnly = false,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.RuleAmountType),
                    DataPropertyName = nameof(CustomerPricingRule.RuleAmountType),
                    DataSource = _model.CustomerPricingRulesEditViewModel.RuleAmountTypes,
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
            dataGridViewCurrentRules.DataSource = _periodRules;
            //
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.PricingRuleTypes,
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.ItemTypes,
                    DisplayMember = nameof(ItemType.Name),
                    ValueMember = nameof(ItemType.Self)
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.Group),
                    DataPropertyName = nameof(CustomerPricingRule.Group),
                    DataSource = _model.CustomerPricingRulesEditViewModel.Groups,
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.Companies,
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
                    DataSource = _model.CustomerPricingRulesEditViewModel.IncrementDecrement,
                    ReadOnly = false,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = nameof(CustomerPricingRule.RuleAmountType),
                    DataPropertyName = nameof(CustomerPricingRule.RuleAmountType),
                    DataSource = _model.CustomerPricingRulesEditViewModel.RuleAmountTypes,
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
            _model.PropsedPricingRules = new BindingList<CustomerPricingRule>();
            dataGridView1.DataSource = _model.PropsedPricingRules;
            dataGridView1.DefaultValuesNeeded += DataGridView1_DefaultValuesNeeded;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
            dataGridView1.RowValidating += DataGridView1_RowValidating;
            dataGridView1.RowValidated += DataGridView1_RowValidated;
            _autoCompleteSource.AddRange(_model.CustomerPricingRulesEditViewModel.ItemsCodes.ToArray());
            //
            SelectedPeriodChanged(0);
            //
            SelectedItemChanged(0,0);
            //

        }
        private void DataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = "";
        }
        private void DataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (grid != null && e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
            {
                switch (_model.PropsedPricingRules[e.RowIndex].RuleType)
                {
                    case CustomerPricingRuleTypes.ItemInCompany:
                        if (_model.PropsedPricingRules[e.RowIndex].Company == null || _model.PropsedPricingRules[e.RowIndex].Company.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "select a company from the list.";
                        }
                        else if (string.IsNullOrEmpty(_model.PropsedPricingRules[e.RowIndex].ItemCode))
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Item Code.";
                        }
                        break;
                    case CustomerPricingRuleTypes.AllItems:
                        if (!string.IsNullOrEmpty(_model.PropsedPricingRules[e.RowIndex].ItemCode))
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "All Items Rule Type Cannot has Item Code!";
                        }
                        break;
                    case CustomerPricingRuleTypes.Company:
                        if (_model.PropsedPricingRules[e.RowIndex].Company == null || _model.PropsedPricingRules[e.RowIndex].Company.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "select a company from the list.";
                        }
                        break;
                    case CustomerPricingRuleTypes.Item:
                        if (string.IsNullOrEmpty(_model.PropsedPricingRules[e.RowIndex].ItemCode))
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Item Code.";
                        }
                        else
                        {
                            if (!_autoCompleteSource.Contains(_model.PropsedPricingRules[e.RowIndex].ItemCode))
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
                        if (_model.PropsedPricingRules[e.RowIndex].Group == null || _model.PropsedPricingRules[e.RowIndex].Group.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Select a Group from the list.";
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemType:
                        if (_model.PropsedPricingRules[e.RowIndex].ItemType == null || _model.PropsedPricingRules[e.RowIndex].ItemType.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Select an Item Type.";
                        }
                        break;
                }
                switch (_model.PropsedPricingRules[e.RowIndex].RuleAmountType)
                {
                    case RuleAmountTypes.Fixed:
                        if (_model.PropsedPricingRules[e.RowIndex].AmountCurrency == null || _model.PropsedPricingRules[e.RowIndex].AmountCurrency.Id == Guid.Empty)
                        {
                            e.Cancel = true;
                            System.Media.SystemSounds.Hand.Play();
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Select Currency";
                        }

                        break;
                    case RuleAmountTypes.Percentage:
                        break;
                }
                if (_model.PropsedPricingRules[e.RowIndex].Amount < 0)
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
            if (index != null && index == dataGridView1.Columns[nameof(CustomerPricingRule.ItemCode)].Index)
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
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                        {
                            _model.PropsedPricingRules[e.RowIndex].Group = _model.CustomerPricingRulesEditViewModel.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].ItemType = _model.CustomerPricingRulesEditViewModel.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.AllItems:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                        {
                            _model.PropsedPricingRules[e.RowIndex].ItemCode = "";
                            _model.PropsedPricingRules[e.RowIndex].Group = _model.CustomerPricingRulesEditViewModel.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].Company = _model.CustomerPricingRulesEditViewModel.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].ItemType = _model.CustomerPricingRulesEditViewModel.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.Company:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                        {
                            _model.PropsedPricingRules[e.RowIndex].ItemCode = "";
                            _model.PropsedPricingRules[e.RowIndex].Group = _model.CustomerPricingRulesEditViewModel.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].ItemType = _model.CustomerPricingRulesEditViewModel.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].Company = _model.CustomerPricingRulesEditViewModel.Companies.Where(c => c.Id != Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.Item:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                        {
                            _model.PropsedPricingRules[e.RowIndex].Group = _model.CustomerPricingRulesEditViewModel.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].Company = _model.CustomerPricingRulesEditViewModel.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].ItemType = _model.CustomerPricingRulesEditViewModel.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemGroup:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = false;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                        {
                            _model.PropsedPricingRules[e.RowIndex].ItemCode = "";
                            _model.PropsedPricingRules[e.RowIndex].Company = _model.CustomerPricingRulesEditViewModel.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].ItemType = _model.CustomerPricingRulesEditViewModel.ItemTypes.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].Group = _model.CustomerPricingRulesEditViewModel.Groups.Where(g => g.Id != Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case CustomerPricingRuleTypes.ItemType:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemCode)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Group)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.Company)].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.ItemType)].ReadOnly = false;
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                        {
                            _model.PropsedPricingRules[e.RowIndex].ItemCode = "";
                            _model.PropsedPricingRules[e.RowIndex].Group = _model.CustomerPricingRulesEditViewModel.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].Company = _model.CustomerPricingRulesEditViewModel.Companies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                            _model.PropsedPricingRules[e.RowIndex].ItemType = _model.CustomerPricingRulesEditViewModel.ItemTypes.Where(t => t.Id != Guid.Empty).FirstOrDefault();
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
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                        {
                            _model.PropsedPricingRules[e.RowIndex].AmountCurrency = _model.Currencies.Where(c => c.Id != Guid.Empty).FirstOrDefault();
                        }
                        break;
                    case RuleAmountTypes.Percentage:
                        dataGridView1.Rows[e.RowIndex].Cells[nameof(CustomerPricingRule.AmountCurrency)].ReadOnly = true;
                        if (e.RowIndex >= 0 && e.RowIndex < _model.PropsedPricingRules.Count)
                            _model.PropsedPricingRules[e.RowIndex].AmountCurrency = _model.Currencies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
                        break;
                }
            }
        }
        private void DataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[nameof(CustomerPricingRule.RuleType)].Value = CustomerPricingRuleTypes.AllItems;
            e.Row.Cells[nameof(CustomerPricingRule.ItemCode)].Value = "";
            e.Row.Cells[nameof(CustomerPricingRule.ItemType)].Value = _model.CustomerPricingRulesEditViewModel.ItemTypes.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Group)].Value = _model.CustomerPricingRulesEditViewModel.Groups.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Company)].Value = _model.CustomerPricingRulesEditViewModel.Companies.Where(g => g.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.IncrementDecrement)].Value = IncrementDecrementTypes.Increment;
            e.Row.Cells[nameof(CustomerPricingRule.RuleAmountType)].Value = RuleAmountTypes.Percentage;
            e.Row.Cells[nameof(CustomerPricingRule.Amount)].Value = 0M;
            e.Row.Cells[nameof(CustomerPricingRule.AmountCurrency)].Value = _model.Currencies.Where(c => c.Id == Guid.Empty).FirstOrDefault();
            e.Row.Cells[nameof(CustomerPricingRule.Customer)].Value = _model.Customers.Where(c => c.Id == Guid.Empty).FirstOrDefault();
        }
        private void cboItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedItemChanged(cboItems.SelectedIndex,cboPeriods.SelectedIndex);
        }

        private void cboPeriods_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedPeriodChanged(cboPeriods.SelectedIndex);
            SelectedItemChanged(cboItems.SelectedIndex,cboPeriods.SelectedIndex);
        }

        private void SelectedPeriodChanged(int idx)
        {
            if(idx >= 0 && idx < _model.AccountingPeriods.Count)
            {
                var periodRules = _accountingPeriodController.FindPricingRules(_model.AccountingPeriods[idx]);
                _periodRules.Clear();
                foreach (var rule in periodRules)
                {
                    _periodRules.Add(rule);
                }
            }

        }
        private void SelectedItemChanged(int idx,int prdIdx)
        {
            if (idx >= 0 && idx < _model.Items.Count && prdIdx >=0 && prdIdx < _model.AccountingPeriods.Count)
            {
                var line = _priceListController.Find(_model.Items[idx], _model.AccountingPeriods[prdIdx]);
                if (line != null)
                {
                    txtCurencyCode.Text = line.CurrencyCode;
                    _model.CurrentCurrencyCode = line.CurrencyCode;
                    txtUnitPrice.Text = line.UnitPrice.Value.ToString("#0.##");
                    _model.CurrentUnitPrice = line.UnitPrice;
                    txtRate.Text = line.CurrencyExchangeRate?.ToString("#0.##") ?? "";
                    _model.CurrentExchangeRate = line.CurrencyExchangeRate;
                    txtTarrif.Text = line.TarrifPercentage?.ToString("#0.##") ?? "";
                    _model.CurrentTarrif = line.TarrifPercentage;
                }
                else
                {
                    txtCurencyCode.Text = "";
                   _model.CurrentCurrencyCode = "";
                    txtUnitPrice.Text = "";
                    _model.CurrentUnitPrice = null;
                    txtRate.Text = "";
                    _model.CurrentExchangeRate = null;
                    txtTarrif.Text = "";
                    _model.CurrentTarrif = null;
                }
            }
        }

        private void txtProposedUnitPrice_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtProposedUnitPrice.Text))
            {
                e.Cancel = true;
                System.Media.SystemSounds.Hand.Play();
                errorProvider1.SetError(txtUnitPrice, "Invalid Unit Price");
            }
            else
            {
                if (decimal.TryParse(txtProposedUnitPrice.Text,out decimal uprice))
                {
                    if (uprice < 0)
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        errorProvider1.SetError(txtProposedUnitPrice, "Invalid Unit Price");
                    }
                }
                else
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    errorProvider1.SetError(txtProposedUnitPrice, "Invalid Unit Price");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtProposedUnitPrice_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtProposedUnitPrice, "");
        }

        private void cboCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboCurrency.SelectedIndex;
            if(idx>=0 && idx < _model.Currencies.Count)
            {
                if (_model.Currencies[idx].Id != _model.CompanyCurrency.Id)
                {
                    txtProposedRate.ReadOnly = false;
                    txtProposedTarrif.ReadOnly = false;
                }
                else
                {
                    _model.ProposedRate = null;
                    _model.PropsedTarrif = null;
                    txtProposedRate.Text = "";
                    txtProposedRate.ReadOnly = true;
                    txtProposedTarrif.Text = "";
                    txtProposedTarrif.ReadOnly = true;
                }
            }
        }

        private void txtProposedRate_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtProposedRate.Text))
            {
                if(decimal.TryParse(txtProposedRate.Text,out decimal propsedRate))
                {
                    if(propsedRate < 0)
                    {
                        e.Cancel = true;
                        errorProvider1.SetError(txtProposedRate, "Invalid Rate.");
                        System.Media.SystemSounds.Hand.Play();
                    }
                }
                else
                {
                    e.Cancel = true;
                    errorProvider1.SetError(txtProposedRate, "Invalid Rate.");
                    System.Media.SystemSounds.Hand.Play();
                }
            }
        }

        private void txtProposedRate_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProposedRate.Text))
            {
                _model.ProposedRate = null;
            }
            else
            {
                _model.ProposedRate = decimal.Parse(txtProposedRate.Text);
            }
        }

        private void txtProposedTarrif_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtProposedTarrif.Text))
            {
                if (decimal.TryParse(txtProposedTarrif.Text,out decimal propsedTarrif))
                {
                    if(propsedTarrif < 0)
                    {
                        e.Cancel = true;
                        errorProvider1.SetError(txtProposedTarrif, "Invalid Tarrif.");
                        System.Media.SystemSounds.Hand.Play();
                    }
                }
                else
                {
                    e.Cancel = true;
                    errorProvider1.SetError(txtProposedTarrif, "Invalid Tarrif.");
                    System.Media.SystemSounds.Hand.Play();
                }
            }
        }

        private void txtProposedTarrif_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProposedTarrif.Text))
            {
                _model.PropsedTarrif = null;
            }
            else
            {
                _model.PropsedTarrif = decimal.Parse(txtProposedTarrif.Text);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_model.ProposedUnitPrice < 0)
            {
                _ = MessageBox.Show(this, "Invalid Proposed Unit Price","Error");
                return;
            }
            try
            {
                Cursor = Cursors.WaitCursor;
                var data = _accountingPeriodController.GenerateSimulationReportData(_model);
                var currentRules = _periodRules.Select(
                    r => new ViewModels.CustomerPricingRuleViewModel()
                    {
                        AccountingPeriodName = r.AccountingPeriod.Name,
                        Amount = r.Amount,
                        AmountType = r.RuleAmountType,
                        CompanyName = r.Company?.Name,
                        GroupName = r.Group?.Name,
                        CustomerName = r.Customer?.Name,
                        IncrementDecrement = r.IncrementDecrement,
                        ItemCode = r.Item?.Code,
                        ItemTypeName = r.ItemType?.Name,
                        RuleType = r.RuleType
                    }).ToList();
                var proposedRules = _model.PropsedPricingRules.Select(r => new ViewModels.CustomerPricingRuleViewModel()
                {
                    AccountingPeriodName = r.AccountingPeriod?.Name,
                    Amount = r.Amount,
                    AmountType = r.RuleAmountType,
                    CompanyName = r.Company?.Name,
                    GroupName = r.Group?.Name,
                    CustomerName = r.Customer?.Name,
                    IncrementDecrement = r.IncrementDecrement,
                    ItemCode = r.Item?.Code,
                    ItemTypeName = r.ItemType?.Name,
                    RuleType = r.RuleType
                }).ToList();
                ReportsForms.SimulationReportForm simulationReportForm = new ReportsForms.SimulationReportForm(data, currentRules, proposedRules);
                simulationReportForm.MdiParent = this.MdiParent;
                simulationReportForm.Show();
                Close();
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
