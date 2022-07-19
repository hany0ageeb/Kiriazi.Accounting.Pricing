using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kiriazi.Accounting.Pricing.Views
{
    public partial class ItemCostView : Form
    {
        private readonly Controllers.AccountingPeriodController _controller;
        private ViewModels.ItemCostedSearchViewModel _model;
        private BindingList<ViewModels.ItemCostedSearchResultViewModel> _results = new BindingList<ViewModels.ItemCostedSearchResultViewModel>();
        public ItemCostView(Controllers.AccountingPeriodController controller)
        {
            _controller = controller;
            _model = _controller.FindItemCosted();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            cboAccountingPeriods.DataBindings.Clear();
            cboAccountingPeriods.DataSource = _model.AccountingPeriods;
            cboAccountingPeriods.DisplayMember = nameof(Models.AccountingPeriod.Name);
            cboAccountingPeriods.ValueMember = nameof(Models.AccountingPeriod.Self);
            cboAccountingPeriods.DataBindings.Add(new Binding(nameof(cboAccountingPeriods.SelectedItem),_model,nameof(_model.AccountingPeriod))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            //
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(_model.Company.Name);
            cboCompanies.ValueMember = nameof(_model.Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_model,nameof(_model.Company))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            //
            cboItems.DataBindings.Clear();
            cboItems.DataSource = _model.Items;
            cboItems.DisplayMember = nameof(_model.Item.Code);
            cboItems.ValueMember = nameof(_model.Item.Self);
            cboItems.DataBindings.Add(new Binding(nameof(cboItems.SelectedItem),_model,nameof(_model.Item))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            //
            cboCustomers.DataBindings.Clear();
            cboCustomers.DataSource = _model.Customers;
            cboCustomers.DisplayMember = nameof(_model.Customer.Name);
            cboCustomers.ValueMember = nameof(_model.Customer.Self);
            cboCustomers.DataBindings.Add(new Binding(nameof(cboCustomers.SelectedItem), _model, nameof(_model.Customer))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            //
            cboGroups.DataBindings.Clear();
            cboGroups.DataSource = _model.Groups;
            cboGroups.DisplayMember = nameof(_model.Group.Name);
            cboGroups.ValueMember = nameof(_model.Group.Self);
            cboGroups.DataBindings.Add(new Binding(nameof(cboGroups.SelectedItem),_model,nameof(_model.Group)) 
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            //
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Accounting Period",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.AccountingPeriodName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.AccountingPeriodName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Company",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.CompanyName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.CompanyName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Customer",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.CustomerName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.CustomerName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Code",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemCode)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Description",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemArabicName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemArabicName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Uom",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemUomCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemArabicName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Type",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemTypeName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemTypeName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Unit Price",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPrice),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPrice)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Unit Price Currency",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPriceCurrencyCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPriceCurrencyCode)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Exchange Rate",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ExchangeRate),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ExchangeRate)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Tarrif",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.Tarrif),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.Tarrif)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Value",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.Value),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.Value)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Value Currency",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ValueCurrencyCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ValueCurrencyCode)
                });
            dataGridView1.DataSource = _results;
            dataGridView1.SelectionChanged += (o, e) =>
            {
                int? index = dataGridView1.CurrentRow?.Index;
                if(index.HasValue && index >= 0 && index < _results.Count)
                {
                    var result = _results[index.Value];
                    if(result.PricingRules!=null && result.PricingRules.Count > 0)
                    {
                        btnShowPricingRules.Enabled = true;
                    }
                    else
                    {
                        btnShowPricingRules.Enabled = false;
                    }
                    if(result.Components!=null && result.Components.Count > 0)
                    {
                        btnShowComponents.Enabled = true;
                    }
                    else
                    {
                        btnShowComponents.Enabled = false;
                    }
                }
            };
        }
        private void Search()
        {
            _results.Clear();
            var results = _controller.FindItemCosted(_model);
            foreach(var result in results)
            {
                _results.Add(result);
            }
            if (_results.Count <= 0)
            {
                btnShowComponents.Enabled = false;
                btnShowPricingRules.Enabled = false;
            }
            else
            {
                btnShowComponents.Enabled = true;
                btnShowPricingRules.Enabled = true;
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Search();
            }
            catch(Common.NoAvailableCurrencyExchangeRateException ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void btnShowPricingRules_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if(index.HasValue && index>=0 && index < _results.Count)
            {
                using(CustomerPriceRuleView customerPriceRuleView = new CustomerPriceRuleView(_results[index.Value].PricingRules))
                {
                    customerPriceRuleView.ShowDialog(this);
                }
            }
        }
        private void btnShowComponents_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if (index.HasValue && index >= 0 && index < _results.Count)
            {
                using (ItemComponentsCostedView customerPriceRuleView = new ItemComponentsCostedView(_results[index.Value].Components))
                {
                    customerPriceRuleView.ShowDialog(this);
                }
            }
        }
    }
}
