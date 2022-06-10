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
        
        
        private bool _hasChanged = false;
        
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
           
            //
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text),_model,nameof(_model.Name)) { DataSourceUpdateMode = DataSourceUpdateMode.OnValidation });
            //
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(_model.Company.Name);
            cboCompanies.ValueMember = nameof(_model.Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem), _model, nameof(_model.Company)) 
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            
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
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dataGridView1.MultiSelect = false;
            dataGridView1.DataSource = _lines;
            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.CellValidated += DataGridView1_CellValidated;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.RowValidating += DataGridView1_RowValidating;
            dataGridView1.RowValidated += DataGridView1_RowValidated;
            dataGridView1.DefaultValuesNeeded += DataGridView1_DefaultValuesNeeded;
            
            _model.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            _lines.ListChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            if (_lines.Count > 0)
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            }
            
        }

        private void DataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Currency"].Value = _model.Company.Currency;
        }

        private void DataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = "";
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = 
                    MessageBox.Show(
                        owner:this,
                        text:"Do you want to save changes?",
                        caption:"Confirm Save",
                        buttons:MessageBoxButtons.YesNoCancel,
                        icon:MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
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
                else if(result == DialogResult.No)
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
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _priceListController.SaveOrUpdate(_model);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors("Name");
                    errorProvider1.SetError(txtName, errors.FirstOrDefault()??"");
                    errors = modelState.GetErrors("CompanyAccountingPeriod");
                    errorProvider1.SetError(cboCompanies, errors.FirstOrDefault()??"");
                    errorProvider1.SetError(cboPeriods, errors.FirstOrDefault()??"");
                    errors = modelState.GetErrors("PriceListLines");
                    errorProvider1.SetError(txtName, errors.FirstOrDefault()??"");
                    for(int i = 0; i < modelState.InnerModelStatesCount; i++)
                    {
                        if (modelState.GetModelState(i).HasErrors)
                        {
                            dataGridView1.Rows[i].ErrorText = modelState.GetModelState(i).GetErrors().FirstOrDefault() ?? "";
                        }
                    }
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtName, "");
                    errorProvider1.SetError(cboCompanies, "");
                    errorProvider1.SetError(cboPeriods, "");
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        row.ErrorText = "";
                    }
                    _hasChanged = false;
                    System.Media.SystemSounds.Beep.Play();
                    return true;
                }
            }
            return true;
        }
        private void DataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(e.RowIndex >= 0 && e.RowIndex < _lines.Count)
            {
                dataGridView1.Rows[e.RowIndex].ErrorText = "";
                if (_lines[e.RowIndex].Item != null)
                {
                    
                    if (_lines[e.RowIndex].UnitPrice >= 0)
                    {
                        if (_lines[e.RowIndex].Currency != null)
                        {
                            if (_lines[e.RowIndex].Currency.Id != _model.Company.Currency.Id)
                            {
                                if(_lines[e.RowIndex].ExchangeRateType == ExchangeRateTypes.System)
                                {
                                    if (_lines[e.RowIndex].CurrencyExchangeRate != null)
                                    {
                                        e.Cancel = true;
                                        dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Currency Exchange Rate.\n";
                                        System.Media.SystemSounds.Hand.Play();
                                    }
                                }
                                else if (_lines[e.RowIndex].ExchangeRateType == ExchangeRateTypes.Manual)
                                {
                                    if (_lines[e.RowIndex].CurrencyExchangeRate == null)
                                    {
                                        e.Cancel = true;
                                        dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Currency Exchange Rate.\n";
                                        System.Media.SystemSounds.Hand.Play();
                                    }
                                }
                                else
                                {
                                    e.Cancel = true;
                                    dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Exchange Rate Type [Manual/System]\n";
                                    System.Media.SystemSounds.Hand.Play();
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(_lines[e.RowIndex].ExchangeRateType))
                                {
                                    e.Cancel = true;
                                    dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Exchange Rate Type.\n";
                                    System.Media.SystemSounds.Hand.Play();
                                }
                                else
                                {
                                    if (_lines[e.RowIndex].CurrencyExchangeRate != null)
                                    {
                                        e.Cancel = true;
                                        dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Currecny Exchange Rate.\n";
                                        System.Media.SystemSounds.Hand.Play();
                                    }
                                }
                            }
                            if (_lines[e.RowIndex].Item.Tarrif != null)
                            {
                                if (!string.IsNullOrEmpty(_lines[e.RowIndex].TarrifType))
                                {
                                    if (_lines[e.RowIndex].TarrifType == ExchangeRateTypes.System)
                                    {
                                        if (_lines[e.RowIndex].TarrrifPercentage != null)
                                        {
                                            e.Cancel = true;
                                            dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Tarrif Percentage Value\n";
                                            System.Media.SystemSounds.Hand.Play();
                                        }
                                    }
                                    else if (_lines[e.RowIndex].TarrifType == ExchangeRateTypes.Manual)
                                    {
                                        if (_lines[e.RowIndex].TarrrifPercentage == null)
                                        {
                                            e.Cancel = true;
                                            dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Tarrif Percentage Value\n";
                                            System.Media.SystemSounds.Hand.Play();
                                        }
                                    }
                                    else
                                    {
                                        e.Cancel = true;
                                        dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Tarrif Type\n";
                                        System.Media.SystemSounds.Hand.Play();
                                    }
                                }
                                else
                                {
                                    e.Cancel = true;
                                    dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Tarrif Type\n";
                                    System.Media.SystemSounds.Hand.Play();
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(_lines[e.RowIndex].TarrifType))
                                {
                                    e.Cancel = true;
                                    dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Tarrif Type\n";
                                    System.Media.SystemSounds.Hand.Play();
                                }
                                else
                                {
                                    if (_lines[e.RowIndex].TarrrifPercentage != null)
                                    {
                                        e.Cancel = true;
                                        dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Tarrif Percentage Value.\n";
                                        System.Media.SystemSounds.Hand.Play();
                                    }
                                }
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Unit Price Currecny\n";
                            System.Media.SystemSounds.Hand.Play();
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                        dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Unit Price\n";
                        System.Media.SystemSounds.Hand.Play();
                    }
                }
                else
                {
                    e.Cancel = true;
                    dataGridView1.Rows[e.RowIndex].ErrorText += "Invalid Item\n";
                    System.Media.SystemSounds.Hand.Play();
                }
            }
        }
        
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["ItemCode"].Index)
            {
                if (e.RowIndex >= 0 && e.RowIndex < _lines.Count)
                {

                }
            }
            if (e.ColumnIndex == dataGridView1.Columns["Currency"].Index)
            {
                if(e.RowIndex >=0 && e.RowIndex < _lines.Count)
                {
                    if(_lines[e.RowIndex].Currency != null && _lines[e.RowIndex].Currency.Id != _model.Company.CurrencyId)
                    {
                        
                        dataGridView1.Rows[e.RowIndex].Cells["ExchangeRateType"].ReadOnly = false;
                        _lines[e.RowIndex].ExchangeRateType = ExchangeRateTypes.System;
                        _lines[e.RowIndex].CurrencyExchangeRate = null;
                        dataGridView1.Rows[e.RowIndex].Cells["CurrencyExchangeRate"].ReadOnly = true;
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].Cells["ExchangeRateType"].ReadOnly = true;
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
                        dataGridView1.Rows[e.RowIndex].Cells["CurrencyExchangeRate"].ReadOnly = true;
                        _lines[e.RowIndex].CurrencyExchangeRate = null;
                    }
                    else if(_lines[e.RowIndex].ExchangeRateType == ExchangeRateTypes.Manual)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells["CurrencyExchangeRate"].ReadOnly = false;
                        _lines[e.RowIndex].CurrencyExchangeRate = _priceListController.FindMaximumCurrencyExchangeRateInPeriod(
                            period: _model.AccountingPeriod,
                            fromCurrency: _lines[e.RowIndex].Currency,
                            toCurrency: _model.Company.Currency);
                    }
                }
            }
            if(e.ColumnIndex == dataGridView1.Columns["TarrifType"].Index)
            {
                if(e.RowIndex >=0 && e.RowIndex < _lines.Count)
                {
                    if(_lines[e.RowIndex].TarrifType == ExchangeRateTypes.System)
                    {
                        if(_lines[e.RowIndex].Item != null && _lines[e.RowIndex].Item.Tarrif != null)
                        {
                            //_lines[e.RowIndex].TarrrifPercentage = _lines[e.RowIndex].Item.TarrifPercentage;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrrifPercentage"].ReadOnly = true;
                        }
                        else
                        {
                            _lines[e.RowIndex].TarrifType = null;
                            _lines[e.RowIndex].TarrrifPercentage = null;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrrifPercentage"].ReadOnly = true;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrifType"].ReadOnly = true;
                        }
                    }
                    else if(_lines[e.RowIndex].TarrifType == ExchangeRateTypes.Manual)
                    {
                        if (_lines[e.RowIndex].Item != null && _lines[e.RowIndex].Item.Tarrif != null)
                        {
                            //_lines[e.RowIndex].TarrrifPercentage = _lines[e.RowIndex].Item.TarrifPercentage;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrrifPercentage"].ReadOnly = false;
                            _lines[e.RowIndex].TarrrifPercentage = _lines[e.RowIndex].Item.TarrifPercentage;
                        }
                        else
                        {
                            _lines[e.RowIndex].TarrifType = null;
                            _lines[e.RowIndex].TarrrifPercentage = null;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrrifPercentage"].ReadOnly = true;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrifType"].ReadOnly = true;
                        }
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
                        if (_lines[e.RowIndex].Item.Tarrif != null)
                        {
                           
                            //_lines[e.RowIndex].TarrifType = ExchangeRateTypes.System;
                            //_lines[e.RowIndex].TarrrifPercentage = _lines[e.RowIndex].Item.TarrifPercentage;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrrifPercentage"].ReadOnly = true;
                        }
                        else
                        {
                            _lines[e.RowIndex].TarrifType = null;
                            _lines[e.RowIndex].TarrrifPercentage = null;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrifType"].ReadOnly = true;
                            dataGridView1.Rows[e.RowIndex].Cells["TarrrifPercentage"].ReadOnly = true;
                        }
                    }
                    else
                    {
                        txtItemName.Text = "";
                        txtItemUom.Text = "";
                        _lines[e.RowIndex].Item = null;
                        _lines[e.RowIndex].TarrifType = null;
                        _lines[e.RowIndex].TarrrifPercentage = null;
                        dataGridView1.Rows[e.RowIndex].Cells["TarrifType"].ReadOnly = true;
                        dataGridView1.Rows[e.RowIndex].Cells["TarrrifPercentage"].ReadOnly = true;
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
                else
                {
                    
                }
            }
            if(e.ColumnIndex == dataGridView1.Columns["UnitPrice"].Index)
            {
                decimal? unitPrice = e.FormattedValue as decimal?;
                if(decimal.TryParse(e.FormattedValue.ToString(),out decimal uprice))
                {
                    if (unitPrice < 0)
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
                    DataPropertyName = nameof(PriceListLine.ItemCode),
                    ReadOnly = false,
                    HeaderText = "Item",
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "UnitPrice",
                    DataPropertyName = nameof(PriceListLine.UnitPrice),
                    ReadOnly = false,
                    HeaderText = "Unit Price",
                    Visible = true
                }
                ,new DataGridViewComboBoxColumn()
                {
                    Name = "Currency",
                    DataPropertyName = nameof(PriceListLine.Currency),
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
                    DataPropertyName = nameof(PriceListLine.ExchangeRateType),
                    DataSource = _model.RateTypes,
                    HeaderText = "Currency Exchange Rate Type",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "CurrencyExchangeRate",
                    DataPropertyName = nameof(PriceListLine.CurrencyExchangeRate),
                    HeaderText = "Exchange Rate",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewComboBoxColumn()
                {
                    Name = "TarrifType",
                    DataPropertyName = nameof(PriceListLine.TarrifType),
                    DataSource = _model.RateTypes,
                    ReadOnly = true,
                    HeaderText = "Tarrif Type",
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "TarrrifPercentage",
                    DataPropertyName = nameof(PriceListLine.TarrrifPercentage),
                    HeaderText = "Tarrrif Percentage(%)",
                    ReadOnly = true,
                    Visible = true
                });
        }
        
        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                e.Cancel = true;
                System.Media.SystemSounds.Hand.Play();
                errorProvider1.SetError(txtItemName, "Please Enter Price List Name (a unique Name To Identify the list.)");
            }
        }
        private void txtName_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtItemName, "");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_hasChanged)
            {
                if (SaveChanges())
                {
                    _ = MessageBox.Show(this, "Price List Saved Successfuly.","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    _hasChanged = false;
                    btnSave.Enabled = false;
                    Close();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
            {
                Close();
            }
        }

        private void PriceListEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && e.CloseReason == CloseReason.UserClosing && dataGridView1.EndEdit())   
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
