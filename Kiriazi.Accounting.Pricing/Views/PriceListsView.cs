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
    public partial class PriceListsView : Form
    {
        private readonly Controllers.PriceListController _priceListController;
        private PriceListSearchViewModel _searchModel;
        private BindingList<PriceListViewModel> _priceListsViews = new BindingList<PriceListViewModel>();
        private BindingList<PriceListLineViewModel> _priceListLines = new BindingList<PriceListLineViewModel>();
        private AutoCompleteStringCollection _autoCompleteSource = new AutoCompleteStringCollection();
        public PriceListsView(Controllers.PriceListController priceListController)
        {
            _priceListController = priceListController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            _searchModel = _priceListController.Find();
            // ...
            _autoCompleteSource.AddRange(_searchModel.ItemsCodes.ToArray());
            txtItemCode.DataBindings.Clear();
            txtItemCode.DataBindings.Add(new Binding(nameof(txtItemCode.Text),_searchModel,nameof(_searchModel.ItemCode))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            txtItemCode.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtItemCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtItemCode.AutoCompleteCustomSource = _autoCompleteSource;
            // ...
            cboPeriods.DataBindings.Clear();
            cboPeriods.DataSource = _searchModel.AccountingPeriods;
            cboPeriods.DisplayMember = "Name";
            cboPeriods.ValueMember = "Self";
            cboPeriods.DataBindings.Add(new Binding(nameof(cboPeriods.SelectedItem),_searchModel,nameof(_searchModel.AccountingPeriod))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            // ...
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.CellContentClick += (o, e) =>
            {
                if(e.RowIndex >= 0 && e.RowIndex < _priceListsViews.Count && e.ColumnIndex == dataGridView1.Columns["View"].Index)
                {
                    PriceListView priceListView = new PriceListView(_priceListsViews[e.RowIndex]);
                    priceListView.MdiParent = this.MdiParent;
                    priceListView.Show();
                }
            };
            InitializeDataGridForHeaders();
            
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }
        private void InitializeDataGridForHeaders()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Name",
                DataPropertyName = nameof(PriceListViewModel.PriceListName),
                Name = nameof(PriceListViewModel.PriceListName),
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "Accounting Period Name",
                DataPropertyName = nameof(PriceListViewModel.AccountingPeriodName),
                Name = nameof(PriceListViewModel.AccountingPeriodName),
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "From Date",
                DataPropertyName = nameof(PriceListViewModel.FromDate),
                Name = nameof(PriceListViewModel.FromDate),
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "To Date",
                DataPropertyName = nameof(PriceListViewModel.ToDate),
                Name = nameof(PriceListViewModel.ToDate),
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "State",
                DataPropertyName = nameof(PriceListViewModel.State),
                Name = nameof(PriceListViewModel.State),
                ReadOnly = true
            },
            new DataGridViewButtonColumn()
            {
                UseColumnTextForButtonValue = true,
                Text = "View",
                Name = "View",
                ReadOnly = true,
                HeaderText = ""
            });
            dataGridView1.Columns[nameof(PriceListViewModel.FromDate)].DefaultCellStyle.Format = "g";
            dataGridView1.Columns[nameof(PriceListViewModel.ToDate)].DefaultCellStyle.Format = "g";
            dataGridView1.DataSource = _priceListsViews;
        }
        private void InitilizeDataGridForLines()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Item Code",
                    Name = nameof(PriceListLineViewModel.ItemCode),
                    DataPropertyName = nameof(PriceListLineViewModel.ItemCode),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Item Name",
                    Name = nameof(PriceListLineViewModel.ItemName),
                    DataPropertyName = nameof(PriceListLineViewModel.ItemName),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Item Uom",
                    Name = nameof(PriceListLineViewModel.ItemUom),
                    DataPropertyName = nameof(PriceListLineViewModel.ItemUom),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Currency",
                    Name = nameof(PriceListLineViewModel.CurrencyCode),
                    DataPropertyName = nameof(PriceListLineViewModel.CurrencyCode),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Exchange Rate",
                    Name = nameof(PriceListLineViewModel.CurrencyExchangeRate),
                    DataPropertyName = nameof(PriceListLineViewModel.CurrencyExchangeRate),
                    ReadOnly = true
                });
            dataGridView1.DataSource = _priceListLines;
        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow currentRow = dataGridView1.CurrentRow;
            if (currentRow != null && currentRow.Index>=0 && currentRow.Index<_priceListsViews.Count)
            {
                if(_priceListController.CanChangePriceList(_priceListsViews[currentRow.Index].Id))
                {
                    btnDelete.Enabled = true;
                    btnEdit.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                    btnEdit.Enabled = false;
                }
            }
            else if (currentRow != null && currentRow.Index >= 0 && currentRow.Index < _priceListLines.Count)
            {
                if (_priceListController.CanChangePriceList(_priceListLines[currentRow.Index].PriceListId.Value))
                {
                    btnDelete.Enabled = false;
                    btnEdit.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                    btnEdit.Enabled = false;
                }
            }
        }
        private void Search()
        {
            if (rbtnHeader.Checked)
            {
                _priceListsViews.Clear();
                _priceListLines.Clear();
                InitializeDataGridForHeaders();
                var priceLists = _priceListController.Find(_searchModel);
                foreach (var plist in priceLists)
                {
                    _priceListsViews.Add(plist);
                }
                if (_priceListsViews.Count > 0)
                {
                    btnNewFromExisting.Enabled = true;
                }
                else
                {
                    btnNewFromExisting.Enabled = false;
                    btnDelete.Enabled = false;
                    btnEdit.Enabled = false;
                }
            }
            else if (rbtnLine.Checked)
            {
                _priceListsViews.Clear();
                _priceListLines.Clear();
                InitilizeDataGridForLines();
                var priceListLines = _priceListController.FindLines(_searchModel);
                foreach(var pline in priceListLines)
                {
                    _priceListLines.Add(pline);
                }
                if (_priceListLines.Count > 0)
                {
                    btnNewFromExisting.Enabled = false;
                }
                else
                {
                    btnNewFromExisting.Enabled = false;
                    btnDelete.Enabled = false;
                    btnEdit.Enabled = false;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Models.AccountingPeriod accountingPeriod = null;
            var accountingPeriods = _priceListController.FindAccountingPeriods();
            if (accountingPeriods.Count > 0)
            {
                using (PeriodSelectorView companySelector = new PeriodSelectorView(accountingPeriods))
                {
                    companySelector.ShowDialog(this);
                    if (companySelector.DialogResult == DialogResult.OK)
                    {
                        accountingPeriod = companySelector.SelectedPeriod;
                    }
                }
                if (accountingPeriod != null)
                {
                    using (PriceListEditView priceListEditor = new PriceListEditView(_priceListController, _priceListController.Add(accountingPeriod)))
                    {
                        priceListEditor.ShowDialog(this);
                        Search();
                    }
                }
            }
            else
            {
                _ = MessageBox.Show(this, "No Open periods to create a price list.");
            }
        }

        private void btnNewFromExisting_Click(object sender, EventArgs e)
        {
            var periods = _priceListController.FindAccountingPeriods();
            if (periods.Count > 0)
            {
                using (PeriodSelectorView companySelector = new PeriodSelectorView(periods))
                {
                    companySelector.ShowDialog(this);
                    if(companySelector.DialogResult == DialogResult.OK && companySelector.SelectedPeriod!=null)
                    {
                        using (PriceListEditView priceListEditor = new PriceListEditView(_priceListController, _priceListController.AddFromExisting(companySelector.SelectedPeriod, _priceListsViews[dataGridView1.CurrentRow.Index].Id)))
                        {
                            priceListEditor.ShowDialog(this);
                            Search();
                        }
                    }
                }
            }
            else
            {
                _ = MessageBox.Show(this, "No Open periods to create a price list.");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (_priceListsViews.Count > 0)
                {
                    var model = _priceListController.Edit(_priceListsViews[dataGridView1.CurrentRow.Index].Id);
                   
                    using (PriceListEditView priceListEditor = new PriceListEditView(_priceListController, model))
                    {
                        priceListEditor.ShowDialog(this);
                        Search();
                    }
                }
                else if (_priceListLines.Count > 0)
                {
                    var model = _priceListController.Edit(_priceListLines[dataGridView1.CurrentRow.Index].PriceListId.Value);
                    var line = model.Lines.Where(l => l.ItemCode == _priceListLines[dataGridView1.CurrentRow.Index].ItemCode).FirstOrDefault();
                    if (line != null)
                    {
                        model.Lines.Remove(line);
                        model.Lines.Insert(0, line);
                    }
                    using (PriceListEditView priceListEditor = new PriceListEditView(_priceListController, model))
                    {
                        priceListEditor.ShowDialog(this);
                        Search();
                    }
                }
            }
            catch(ArgumentException ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_priceListsViews.Count > 0)
            {
                int? index = dataGridView1.CurrentRow?.Index;
                if (index != null && index >= 0 && index < _priceListsViews.Count)
                {
                    string message = _priceListController.Delete(_priceListsViews[dataGridView1.CurrentRow.Index].Id);
                    if (!string.IsNullOrEmpty(message))
                    {
                        _ = MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        Search();
                    }
                }
            }
        }
    }
}
