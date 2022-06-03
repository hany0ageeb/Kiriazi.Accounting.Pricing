using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.ViewModels;

namespace Kiriazi.Accounting.Pricing.Views
{
    public partial class PriceListEditView : Form
    {
        private readonly PriceListController _priceListController;
        private PriceListEditViewModel _model;
        private readonly AutoCompleteStringCollection _autoCompleteSource = new AutoCompleteStringCollection();
        private BindingList<PriceListLine> _lines ;
        public PriceListEditView(PriceListController priceListController)
        {
            _priceListController = priceListController;
            _model = _priceListController.Add();
            _autoCompleteSource.AddRange(_model.ItemsCodes.ToArray());
            _lines = new BindingList<PriceListLine>(_model.Lines);
            InitializeComponent();
            DefineGridColumns();
            Initialize();
        }
        public PriceListEditView(PriceListController priceListController,PriceListEditViewModel model)
        {
            _priceListController = priceListController;
            _model = model;
            _autoCompleteSource.AddRange(_model.ItemsCodes.ToArray());
            _lines = new BindingList<PriceListLine>(_model.Lines);
            InitializeComponent();
            DefineGridColumns();
            Initialize();
        }
        private void Initialize()
        {
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(_model.Company.Name);
            cboCompanies.ValueMember = nameof(_model.Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem), _model, nameof(_model.Company)) 
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            cboCompanies.SelectedIndexChanged += CboCompanies_SelectedIndexChanged;
            //
            cboPeriods.DataBindings.Clear();
            cboPeriods.DataSource = _model.AccountingPeriods;
            cboPeriods.DisplayMember = nameof(_model.AccountingPeriod.Name);
            cboPeriods.ValueMember = nameof(_model.AccountingPeriod.Self);
            cboPeriods.DataBindings.Add(new Binding(nameof(cboPeriods.SelectedItem),_model,nameof(_model.AccountingPeriod)) 
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.MultiSelect = false;
            dataGridView1.DataSource = _lines;
            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.CellValidated += DataGridView1_CellValidated;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            _lines.AddingNew += _lines_AddingNew;
        }
        private void _lines_AddingNew(object sender, AddingNewEventArgs e)
        {
            var newLine = e.NewObject as PriceListLine;
            if (newLine != null)
            {
                newLine.Currency = _model.Company.Currency;
            }
        }
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Currency"].Index)
            {
                if(e.RowIndex >=0 && e.RowIndex < _lines.Count)
                {
                    if(_lines[e.RowIndex].Currency.Id != _model.Company.CurrencyId)
                    {
                        dataGridView1.Columns["ExchangeRateType"].Visible = true;
                        dataGridView1.Columns["ExchangeRateType"].ReadOnly = false;
                        dataGridView1.Columns["CurrencyExchangeRate"].Visible = true;
                        _lines[e.RowIndex].ExchangeRateType = ExchangeRateTypes.System;
                        _lines[e.RowIndex].ExchangeRateType = null;
                        dataGridView1.Columns["CurrencyExchangeRate"].ReadOnly = true;
                    }
                    else
                    {
                        dataGridView1.Columns["ExchangeRateType"].Visible = false;
                        dataGridView1.Columns["ExchangeRateType"].ReadOnly = true;
                        dataGridView1.Columns["CurrencyExchangeRate"].Visible = false;
                        _lines[e.RowIndex].CurrencyExchangeRate = null;
                        _lines[e.RowIndex].ExchangeRateType = null;
                    }
                }
            }
            if(e.ColumnIndex == dataGridView1.Columns["ExchangeRateType"].Index)
            {
                if (e.RowIndex >= 0 && e.RowIndex < _lines.Count)
                {
                    if(_lines[e.RowIndex].ExchangeRateType == ExchangeRateTypes.System)
                    {
                        dataGridView1.Columns["CurrencyExchangeRate"].ReadOnly = true;
                        _lines[e.RowIndex].CurrencyExchangeRate = null;
                    }
                    else if(_lines[e.RowIndex].ExchangeRateType == ExchangeRateTypes.Manual)
                    {
                        dataGridView1.Columns["CurrencyExchangeRate"].ReadOnly = false;
                    }
                }
            }
        }
        private void DataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
            if (e.ColumnIndex == dataGridView1.Columns["ItemCode"].Index)
            {
                if (e.RowIndex >= 0 && e.RowIndex < _lines.Count)
                {
                    Item item = _priceListController.FindItemByCode(_lines[e.RowIndex].ItemCode);
                    if (item != null)
                    {
                        txtItemName.Text = item.ArabicName;
                        txtItemUom.Text = item.Uom.Name;
                        _lines[e.RowIndex].Item = item;
                        _lines[e.RowIndex].TarrifType = ExchangeRateTypes.System;
                        _lines[e.RowIndex].TarrrifPercentage = null;
                    }
                    else
                    {
                        txtItemName.Text = "";
                        txtItemUom.Text = "";

                        _lines[e.RowIndex].Item = null;
                        _lines[e.RowIndex].TarrifType = null;
                        _lines[e.RowIndex].TarrrifPercentage = null;

                        dataGridView1.Columns["TarrifType"].ReadOnly = true;
                        dataGridView1.Columns["TarrifType"].Visible = false;

                        dataGridView1.Columns["TarrrifPercentage"].ReadOnly = true;
                        dataGridView1.Columns["TarrrifPercentage"].Visible = false;
                    }
                }
            }
        }
        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if(e.ColumnIndex == dataGridView1.Columns["ItemCode"].Index)
            {
                string itemCode = e.FormattedValue as string;
                if (!_autoCompleteSource.Contains(itemCode))
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Invalid Item Code";       
                }
            }
            if(e.ColumnIndex == dataGridView1.Columns["UnitPrice"].Index)
            {
                decimal? unitPrice = e.FormattedValue as decimal?;
                if (unitPrice != null)
                {
                    if(unitPrice < 0)
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Invalid Unit Price[Enter Value >= 0]";
                    }
                }
                else
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Invalid Unit Price";
                }
            }
        }
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox textBox = e.Control as TextBox;
            if (textBox != null)
            {
                if(dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns["ItemCode"].Index)
                {
                    textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    textBox.AutoCompleteCustomSource = _autoCompleteSource;
                }
            }
        }
        private void DefineGridColumns()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    Name = "ItemCode",
                    DataPropertyName = "ItemCode",
                    ReadOnly = false,
                    HeaderText = "Item",
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "UnitPrice",
                    DataPropertyName = "UnitPrice",
                    ReadOnly = false,
                    HeaderText = "Unit Price",
                    Visible = true
                }
                ,new DataGridViewComboBoxColumn()
                {
                    Name = "Currency",
                    DataPropertyName = "Currency",
                    DisplayMember = "Code",
                    ValueMember = "Self",
                    DataSource = _model.Currencies,
                    HeaderText = "Currency",
                    ReadOnly = false,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = "ExchangeRateType",
                    DataPropertyName = "ExchangeRateType",
                    DataSource = _model.RateTypes,
                    HeaderText = "Currency Exchange Rate Type",
                    ReadOnly = true,
                    Visible = false
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "CurrencyExchangeRate",
                    DataPropertyName = "CurrencyExchangeRate",
                    HeaderText = "Exchange Rate",
                    ReadOnly = true,
                    Visible = false
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = "TarrifType",
                    DataPropertyName = "TarrifType",
                    DataSource = _model.RateTypes,
                    ReadOnly = true,
                    HeaderText = "Tarrif Type",
                    Visible = false
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "TarrrifPercentage",
                    DataPropertyName = "TarrrifPercentage",
                    HeaderText = "Tarrrif Percentage(%)",
                    ReadOnly = true,
                    Visible = false
                });
        }
        private void CboCompanies_SelectedIndexChanged(object sender, EventArgs e)
        {
            Company company = cboCompanies.SelectedItem as Company;
            if (company != null)
            {
                _model.AccountingPeriods = _priceListController.FindCompanyOpenedAccountingPeriods(company);
                cboPeriods.DataBindings.Clear();
                cboPeriods.DataSource = _model.AccountingPeriods;
                cboPeriods.DisplayMember = nameof(_model.AccountingPeriod.Name);
                cboPeriods.ValueMember = nameof(_model.AccountingPeriod.Self);
                cboPeriods.DataBindings.Add(new Binding(nameof(cboPeriods.SelectedItem), _model, nameof(_model.AccountingPeriod))
                {
                    DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
                });
                if(_model.AccountingPeriods.Count > 0)
                {
                    _model.AccountingPeriod = _model.AccountingPeriods[0];
                }
                else
                {
                    _model.AccountingPeriod = null;
                    cboPeriods.Items.Clear();
                    cboPeriods.ResetText();
                }
            }
            else
            {
                _model.AccountingPeriods.Clear();
                _model.AccountingPeriod = null;
                cboPeriods.Items.Clear();
                cboPeriods.ResetText();
            }
        }
    }
}
