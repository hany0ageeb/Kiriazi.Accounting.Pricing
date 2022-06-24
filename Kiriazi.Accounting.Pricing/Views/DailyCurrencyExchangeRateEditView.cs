using Kiriazi.Accounting.Pricing.Controllers;
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
    public partial class DailyCurrencyExchangeRateEditView : Form
    {
        private readonly CurrencyExchangeRateController _controller;
        private BindingList<DailyCurrencyExchangeRateViewModel> _lines = new BindingList<DailyCurrencyExchangeRateViewModel>();
        private IList<Models.AccountingPeriod> _periods;
        private bool _hasChanged = false;
        public DailyCurrencyExchangeRateEditView(CurrencyExchangeRateController controller,IList<Models.AccountingPeriod> accountingPeriods)
            : this(controller, accountingPeriods, null)
        {
            
        }
        public DailyCurrencyExchangeRateEditView(
            CurrencyExchangeRateController controller,
            IList<Models.AccountingPeriod> accountingPeriods,
            IList<DailyCurrencyExchangeRateViewModel> lines = null)
        {
            _controller = controller;
            _periods = accountingPeriods;
            if (lines == null || lines.Count==0)
            {
                lines = _controller.Add();
            }
            foreach(var l in lines)
            {
                _lines.Add(l);
            }
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            cboPeriods.ValueMember = nameof(Models.AccountingPeriod.Self);
            cboPeriods.DisplayMember = nameof(Models.AccountingPeriod.Name);
            cboPeriods.DataSource = _periods;
            cboPeriods.SelectedIndexChanged += (o, e) =>
            {
                Models.AccountingPeriod accountingPeriod = cboPeriods.SelectedItem as Models.AccountingPeriod;
                if (accountingPeriod != null)
                {
                    foreach (var line in _lines)
                    {
                        line.AccountingPeriod = accountingPeriod;
                    }
                }
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    HeaderText ="From Currency",
                    Name = nameof(DailyCurrencyExchangeRateViewModel.FromCurrencyCode),
                    DataPropertyName = nameof(DailyCurrencyExchangeRateViewModel.FromCurrencyCode),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "To Currency",
                    Name = nameof(DailyCurrencyExchangeRateViewModel.ToCurrencyCode),
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.ToCurrencyCode),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Rate",
                    Name = nameof(DailyCurrencyExchangeRateViewModel.Rate),
                    DataPropertyName = nameof(DailyCurrencyExchangeRateViewModel.Rate),
                    ReadOnly = false
                });
            dataGridView1.DataSource = _lines;
            dataGridView1.RowValidating += (o, e) =>
            {
                if(e.RowIndex>=0 && e.RowIndex < _lines.Count)
                {
                    if(_lines[e.RowIndex].Rate <= 0)
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Rate. Enter Value greater than zero.";
                    }
                }
            };
            dataGridView1.RowValidated += (o, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < _lines.Count)
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "";
                }
            };
            _lines.ListChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            btnSave.Enabled = false;
            _hasChanged = false;
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _controller.SaveOrUpdate(_lines);
                if (modelState.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(var err in modelState.GetErrors())
                    {
                        sb.AppendLine(err);
                    }
                    for(int idx = 0; idx < modelState.InnerModelStatesCount; idx++)
                    {
                        foreach(var err in modelState.GetModelState(idx).GetErrors())
                        {
                            sb.AppendLine(err);
                        }
                    }
                    using(Views.ImportErrorsView errorsView = new ImportErrorsView(sb.ToString()))
                    {
                        errorsView.ShowDialog(this);
                    }
                    return false;
                }
                else
                {
                    _hasChanged = false;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(
                    owner: this, 
                    text: "Do you want to save changes?",
                    caption: "Confirm Save", 
                    buttons: MessageBoxButtons.YesNoCancel,
                    icon: MessageBoxIcon.Question);
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
                else if (result == DialogResult.No)
                {
                    _hasChanged = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_hasChanged)
            {
                if (SaveChanges())
                {
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
        private void DailyCurrencyExchangeRateEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
