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
    public partial class PeriodSelectorView : Form
    {
        private readonly IList<Models.AccountingPeriod> _periods;
        private bool _OkClicked = false;
        public PeriodSelectorView(IList<Models.AccountingPeriod> periods)
        {
            _periods = periods;
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
            lstCompanies.Columns.Add("Description", 300,HorizontalAlignment.Center);
            //lstCompanies.Columns.Add("FromDate", 300, HorizontalAlignment.Center);
            foreach (var period in _periods)
            {
                ListViewItem item = new ListViewItem(period.Name, period.Description);
                lstCompanies.Items.Add(item);
            }
            _ = lstCompanies.SelectedIndices.Add(0);
        }
        public Models.AccountingPeriod SelectedPeriod { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedPeriod = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CompanySelectorView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !_OkClicked)
            {
                DialogResult = DialogResult.Cancel;
                SelectedPeriod = null;
            }
                
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lstCompanies.SelectedIndices.Count > 0)
            {
                SelectedPeriod = _periods[lstCompanies.SelectedIndices[0]];
                _OkClicked = true;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                SelectedPeriod = null;
                _OkClicked = true;
                this.DialogResult = DialogResult.Cancel;
            }
            Close();
        }
    }
}
