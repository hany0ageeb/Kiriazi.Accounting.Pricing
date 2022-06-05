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
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _searchModel.Companies;
            cboCompanies.DisplayMember = "Name";
            cboCompanies.ValueMember = "self";
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_searchModel,nameof(_searchModel.Company))
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
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
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Name",
                DataPropertyName = "PriceListName",
                Name = "PriceListName",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "Company Name",
                DataPropertyName = "CompanyName",
                Name = "CompanyName",
                ReadOnly = true

            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "Accounting Period Name",
                DataPropertyName = "AccountingPeriodName",
                Name = "AccountingPeriodName",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "From Date",
                DataPropertyName = "FromDate",
                Name = "FromDate",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "To Date",
                DataPropertyName = "ToDate",
                Name = "ToDate",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "State",
                DataPropertyName = "State",
                Name = "State",
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
            dataGridView1.Columns["FromDate"].DefaultCellStyle.Format = "g";
            dataGridView1.Columns["ToDate"].DefaultCellStyle.Format = "g";
            dataGridView1.DataSource = _priceListsViews;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
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
        }
        private void Search()
        {
            var priceLists = _priceListController.Find(_searchModel);
            _priceListsViews.Clear();
            foreach(var plist in priceLists)
            {
                _priceListsViews.Add(plist);
            }
            if(_priceListsViews.Count > 0)
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
            Models.Company selectedCompany = null;
            var companies = _priceListController.FindCompanies();
            if (companies.Count > 0)
            {
                using (CompanySelectorView companySelector = new CompanySelectorView(companies))
                {
                    companySelector.ShowDialog(this);
                    if (companySelector.DialogResult == DialogResult.OK)
                    {
                        selectedCompany = companySelector.SelectedCompany;
                    }
                }
                if (selectedCompany != null)
                {
                    using (PriceListEditView priceListEditor = new PriceListEditView(_priceListController, _priceListController.Add(selectedCompany)))
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
            var companies = _priceListController.FindCompanies();
            if (companies.Count > 0)
            {
                using (CompanySelectorView companySelector = new CompanySelectorView(_priceListController.FindCompanies()))
                {
                    companySelector.ShowDialog(this);
                    if(companySelector.DialogResult == DialogResult.OK && companySelector.SelectedCompany!=null)
                    {
                        using (PriceListEditView priceListEditor = new PriceListEditView(_priceListController, _priceListController.AddFromExisting(companySelector.SelectedCompany, _priceListsViews[dataGridView1.CurrentRow.Index].Id)))
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
                using (PriceListEditView priceListEditor = new PriceListEditView(_priceListController, _priceListController.Edit(_priceListsViews[dataGridView1.CurrentRow.Index].Id)))
                {
                    priceListEditor.ShowDialog(this);
                    Search();
                }
            }
            catch(ArgumentException ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if (index!=null && index>=0 && index < _priceListsViews.Count)
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
