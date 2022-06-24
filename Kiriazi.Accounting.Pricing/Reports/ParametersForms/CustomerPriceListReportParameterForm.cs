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
    public partial class CustomerPriceListReportParameterForm : Form
    {
        private Controllers.CustomerPriceListController _companyController;
        private ViewModels.CustomerPriceListSeachViewModel _model;
        public CustomerPriceListReportParameterForm(Controllers.CustomerPriceListController companyController)
        {
            _companyController = companyController;
            _model = _companyController.FindCustomerPriceList();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            _model.Companies.RemoveAt(0);
            _model.Customers.RemoveAt(0);
            if (_model.Companies.Count > 0)
            {
                _model.Company = _model.Companies[0];
            }
            else
            {
                _model.Company = null;
            }
            if (_model.Customers.Count > 0)
            {
                _model.Customer = _model.Customers[0];
            }
            else
            {
                _model.Customer = null;
            }
            if(_model.AccountingPeriods.Count > 0)
            {
                _model.AccountingPeriod = _model.AccountingPeriods[0];
            }
            else
            {
                _model.AccountingPeriod = null;
            }
            //
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(Models.Company.Name);
            cboCompanies.ValueMember = nameof(Models.Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_model,nameof(_model.Company))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            //
            cboCustomers.DataBindings.Clear();
            cboCustomers.DataSource = _model.Customers;
            cboCustomers.DisplayMember = nameof(Models.Customer.Name);
            cboCustomers.ValueMember = nameof(Models.Customer.Self);
            cboCustomers.DataBindings.Add(new Binding(nameof(cboCustomers.SelectedItem),_model,nameof(_model.Customer))
            { 
                DataSourceUpdateMode = DataSourceUpdateMode.OnValidation
            });
            //
            cboAccountingPeriods.DataBindings.Clear();
            cboAccountingPeriods.DataSource = _model.AccountingPeriods;
            cboAccountingPeriods.DisplayMember = nameof(Models.AccountingPeriod.Name);
            cboAccountingPeriods.ValueMember = nameof(Models.AccountingPeriod.Self);
            cboAccountingPeriods.DataBindings.Add(new Binding(nameof(cboAccountingPeriods.SelectedItem), _model, nameof(_model.AccountingPeriod)));
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (_model.AccountingPeriod == null)
                {
                    _ = MessageBox.Show(this, "No Accounting Period Available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                Cursor = Cursors.WaitCursor;
                IList<ViewModels.CustomerPriceListViewModel> data = _companyController.FindCustomerPriceList(_model);
                Reports.ReportsForms.CustomerPriceListReportForm customerPriceListReportForm = new ReportsForms.CustomerPriceListReportForm(data);
                customerPriceListReportForm.MdiParent = this.MdiParent;
                customerPriceListReportForm.Show();
                Close();
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
