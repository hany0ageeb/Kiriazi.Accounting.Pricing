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
    public partial class CompanySelectorView : Form
    {
        private readonly IList<Models.Company> _companies;
        private bool _OkClicked = false;
        public CompanySelectorView(IList<Models.Company> companies)
        {
            _companies = companies;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            lstCompanies.View = View.Details;
            lstCompanies.MultiSelect = false;
            lstCompanies.FullRowSelect = true;
            lstCompanies.Columns.Clear();
            lstCompanies.Columns.Add("Name",100,HorizontalAlignment.Center);
            lstCompanies.Columns.Add("Description",300,HorizontalAlignment.Center);
            foreach(var company in _companies)
            {
                ListViewItem item = new ListViewItem(company.Name, company.Description);
                lstCompanies.Items.Add(item);
            }
            _ = lstCompanies.SelectedIndices.Add(0);
        }
        public Models.Company SelectedCompany { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedCompany = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CompanySelectorView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !_OkClicked)
            {
                DialogResult = DialogResult.Cancel;
                SelectedCompany = null;
            }
                
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lstCompanies.SelectedIndices.Count > 0)
            {
                SelectedCompany = _companies[lstCompanies.SelectedIndices[0]];
                _OkClicked = true;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                SelectedCompany = null;
                _OkClicked = true;
                this.DialogResult = DialogResult.Cancel;
            }
            Close();
        }
    }
}
