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
            });
            dataGridView1.Columns["FromDate"].DefaultCellStyle.Format = "g";
            dataGridView1.Columns["ToDate"].DefaultCellStyle.Format = "g";
            dataGridView1.DataSource = _priceListsViews;
        }
        private void Search()
        {
            var priceLists = _priceListController.Find(_searchModel);
            _priceListsViews.Clear();
            foreach(var plist in priceLists)
            {
                _priceListsViews.Add(plist);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
    }
}
