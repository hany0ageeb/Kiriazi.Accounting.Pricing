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
        public DailyCurrencyExchangeRateEditView(CurrencyExchangeRateController controller)
            : this(controller,null)
        {

        }
        public DailyCurrencyExchangeRateEditView(CurrencyExchangeRateController controller,IList<DailyCurrencyExchangeRateViewModel> lines = null)
        {
            _controller = controller;
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
        }
    }
}
