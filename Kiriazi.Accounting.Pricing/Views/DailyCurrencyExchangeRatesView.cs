using Microsoft.Extensions.DependencyInjection;
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
    public partial class DailyCurrencyExchangeRatesView : Form
    {
        private readonly Controllers.CurrencyExchangeRateController _controller;
        private BindingList<ViewModels.DailyCurrencyExchangeRateViewModel> _lines = new BindingList<ViewModels.DailyCurrencyExchangeRateViewModel>();
        public DailyCurrencyExchangeRatesView(Controllers.CurrencyExchangeRateController controller)
        {
            _controller = controller;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            cboPeriods.DisplayMember = nameof(Models.AccountingPeriod.Name);
            cboPeriods.ValueMember = nameof(Models.AccountingPeriod.Self);
            cboPeriods.DataSource = _controller.Find();
            cboPeriods.SelectedIndexChanged += (o, e) =>
            {
                Search();
            };
            // ....
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn() 
                { 
                    HeaderText = "From Currency",
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.FromCurrencyCode),
                    Name = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.FromCurrencyCode),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "To Currency",
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.ToCurrencyCode),
                    Name = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.ToCurrencyCode),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Rate",
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.Rate),
                    Name = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.Rate),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Accounting Period",
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.AccountingPeriodName),
                    Name = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.AccountingPeriodName),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "From Date",
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.FromDate),
                    Name = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.FromDate),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "To Date",
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.ToDate),
                    Name = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.ToDate),
                    ReadOnly = true
                });
            dataGridView1.Columns[nameof(ViewModels.DailyCurrencyExchangeRateViewModel.Rate)].DefaultCellStyle.Format = "##0.####";
            dataGridView1.Columns[nameof(ViewModels.DailyCurrencyExchangeRateViewModel.FromDate)].DefaultCellStyle.Format = "g";
            dataGridView1.Columns[nameof(ViewModels.DailyCurrencyExchangeRateViewModel.ToDate)].DefaultCellStyle.Format = "g";
            dataGridView1.DataSource = _lines;
        }
        private void Search()
        {
            Models.AccountingPeriod accountingPeriod = cboPeriods.SelectedItem as Models.AccountingPeriod;
            _lines.Clear();
            var lines = _controller.Find(accountingPeriod);
            foreach (var line in lines)
            {
                _lines.Add(line);
            }
            if (_lines.Count > 0)
            {
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            var accountingPeriods = _controller.FindAvailableAccountingPeriods();
            if (accountingPeriods.Count == 0)
            {
                _ = MessageBox.Show(this, $"No Accounting Period Available.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            using (DailyCurrencyExchangeRateEditView editView = new DailyCurrencyExchangeRateEditView(_controller,accountingPeriods))
            {
                editView.ShowDialog(this);
                string selectedItem = cboPeriods.SelectedItem as string;
                cboPeriods.DataSource = _controller.Find();
                cboPeriods.SelectedItem = selectedItem;
                if (_lines.Count > 0)
                {
                    Search();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if(index!=null && index >= 0 && index < _lines.Count)
            {
                var lines = _controller.Edit(_lines[index.Value].AccountingPeriod);
                var availablePeriods = _controller.FindAvailableAccountingPeriods();
                if (!availablePeriods.Contains(_lines[index.Value].AccountingPeriod))
                {
                    availablePeriods.Insert(0, _lines[index.Value].AccountingPeriod);
                }
                using(DailyCurrencyExchangeRateEditView editView = new DailyCurrencyExchangeRateEditView(_controller, availablePeriods,lines))
                {
                    editView.ShowDialog(this);
                    Models.AccountingPeriod selectedItem = cboPeriods.SelectedItem as Models.AccountingPeriod;
                    cboPeriods.DataSource = _controller.Find();
                    cboPeriods.SelectedItem = selectedItem;
                    if (_lines.Count > 0)
                    {
                        Search();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if (index != null && index >= 0 && index < _lines.Count)
            {
                Models.AccountingPeriod accountingPeriod = _lines[index.Value].AccountingPeriod;
                DialogResult result = MessageBox.Show(
                    this, 
                    $"Are you Sure you want to delete Currency Exchange Rates For Accounting Period {accountingPeriod.Name} ?", 
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string message = _controller.DeleteCurrencyExchangeRates(accountingPeriod.Id);
                    if (message != string.Empty)
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
