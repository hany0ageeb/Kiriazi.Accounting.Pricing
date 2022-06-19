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
    public partial class CustomerPriceListSearchView : Form
    {
        private readonly CompanyController _companyController;
        private CustomerPriceListSeachViewModel _model;
        public CustomerPriceListSearchView(CompanyController companyController)
        {
            _companyController = companyController;
            _model = _companyController.FindCustomerPriceList();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            //pickPriceListDate.MaxDate = DateTime.Now;
            pickPriceListDate.DataBindings.Clear();
            pickPriceListDate.DataBindings.Add(new Binding(nameof(pickPriceListDate.Value),_model,nameof(_model.Date))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            //
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_model,nameof(_model.Company))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(Company.Name);
            cboCompanies.ValueMember = nameof(Company.Self);
            //
            cboCustomers.DataBindings.Clear();
            cboCustomers.DataBindings.Add(new Binding(nameof(cboCustomers.SelectedItem),_model,nameof(_model.Customer))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            cboCustomers.DataSource = _model.Customers;
            cboCustomers.DisplayMember = nameof(Customer.Name);
            cboCustomers.ValueMember = nameof(Customer.Self);
            //
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var lines = _companyController.FindCustomerPriceList(_model);
                CustomerPriceListsView customerPriceListsView = new CustomerPriceListsView(lines);
                customerPriceListsView.MdiParent = this.MdiParent;
                customerPriceListsView.Show();
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
