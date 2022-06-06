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
            cboDates.DataSource = _controller.Find();
            cboDates.SelectedIndexChanged += (o, e) =>
            {
                string dateAsString = cboDates.SelectedItem as string;
                _lines.Clear();
                var lines = _controller.Find(dateAsString);
                foreach(var line in lines)
                {
                    _lines.Add(line);
                }
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
                    HeaderText = "Date",
                    DataPropertyName = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.Date),
                    Name = nameof(ViewModels.DailyCurrencyExchangeRateViewModel.Date),
                    ReadOnly = true
                });
            dataGridView1.Columns[nameof(ViewModels.DailyCurrencyExchangeRateViewModel.Rate)].DefaultCellStyle.Format = "##0.####";
            dataGridView1.Columns[nameof(ViewModels.DailyCurrencyExchangeRateViewModel.Date)].DefaultCellStyle.Format = "g";
            dataGridView1.DataSource = _lines;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (DailyCurrencyExchangeRateEditView editView = Program.ServiceProvider.GetRequiredService<DailyCurrencyExchangeRateEditView>())
            {
                editView.ShowDialog(this);
            }
        }
    }
}
