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
    public partial class ItemGroupReportParametersForm : Form
    {
        private readonly Controllers.GroupController _groupController;
        private ViewModels.GroupSearchViewModel _searchModel;
        public ItemGroupReportParametersForm(Controllers.GroupController groupController)
        {
            _groupController = groupController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            _searchModel = _groupController.Search();
            // ...
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _searchModel.Companies;
            cboCompanies.DisplayMember = nameof(_searchModel.Company.Name);
            cboCompanies.ValueMember = nameof(_searchModel.Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_searchModel,nameof(_searchModel.Company))
            {

            });
            //
            cboCustomers.DataBindings.Clear();
            cboCustomers.DataSource = _searchModel.Customers;
            cboCustomers.DisplayMember = nameof(_searchModel.Customer.Name);
            cboCustomers.ValueMember = nameof(_searchModel.Customer.Self);
            cboCustomers.DataBindings.Add(new Binding(nameof(cboCustomers.SelectedItem),_searchModel,nameof(_searchModel.Customer))
            {

            });
            //
            cboGroups.DataBindings.Clear();
            cboGroups.DataSource = _searchModel.Groups;
            cboGroups.DisplayMember = nameof(_searchModel.Group.Name);
            cboGroups.ValueMember = nameof(_searchModel.Group.Self);
            cboGroups.DataBindings.Add(new Binding(nameof(cboGroups.SelectedItem),_searchModel,nameof(_searchModel.Group))
            {

            });
            //
            cboPeriods.DataBindings.Clear();
            cboPeriods.DataSource = _searchModel.AccountingPeriods;
            cboPeriods.DisplayMember = nameof(_searchModel.AccountingPeriod.Name);
            cboPeriods.ValueMember = nameof(_searchModel.AccountingPeriod.Self);
            cboPeriods.DataBindings.Add(new Binding(nameof(cboPeriods.SelectedItem), _searchModel, nameof(_searchModel.AccountingPeriod))
            {

            });
            //
            cboItemTypes.DataBindings.Clear();
            cboItemTypes.DataSource = _searchModel.ItemTypes;
            cboItemTypes.DisplayMember = nameof(_searchModel.ItemType.Name);
            cboItemTypes.ValueMember = nameof(_searchModel.ItemType.Self);
            cboItemTypes.DataBindings.Add(new Binding(nameof(cboItemTypes.SelectedItem), _searchModel, nameof(_searchModel.ItemType))
            {

            });
            //
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var data = _groupController.Search(_searchModel);
            ReportsForms.ItemGroupReportForm itemGroupReportForm = new ReportsForms.ItemGroupReportForm(data);
            itemGroupReportForm.MdiParent = this.MdiParent;
            itemGroupReportForm.Show();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
