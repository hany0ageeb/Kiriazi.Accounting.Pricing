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
    public partial class ItemComponentsCostedView : Form
    {
        private BindingList<ViewModels.ItemCostedSearchResultViewModel> _results;
        public ItemComponentsCostedView(IList<ViewModels.ItemCostedSearchResultViewModel> results)
        {
            _results = new BindingList<ViewModels.ItemCostedSearchResultViewModel>(results);
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            //
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Accounting Period",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.AccountingPeriodName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.AccountingPeriodName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Company",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.CompanyName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.CompanyName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Customer",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.CustomerName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.CustomerName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Code",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemCode)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Description",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemArabicName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemArabicName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Uom",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemUomCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemArabicName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Type",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemTypeName),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ItemTypeName)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Unit Price",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPrice),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPrice)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Unit Price Currency",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPriceCurrencyCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.UnitPriceCurrencyCode)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Quantity",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.Quantity),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.Quantity)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Exchange Rate",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ExchangeRate),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ExchangeRate)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Tarrif",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.Tarrif),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.Tarrif)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Value",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.Value),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.Value)
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Value Currency",
                    DataPropertyName = nameof(ViewModels.ItemCostedSearchResultViewModel.ValueCurrencyCode),
                    Name = nameof(ViewModels.ItemCostedSearchResultViewModel.ValueCurrencyCode)
                });
            dataGridView1.DataSource = _results;
            dataGridView1.SelectionChanged += (o, e) =>
            {
                int? index = dataGridView1.CurrentRow?.Index;
                if (index.HasValue && index >= 0 && index < _results.Count)
                {
                    var result = _results[index.Value];
                    if (result.PricingRules != null && result.PricingRules.Count > 0)
                    {
                        btnShowPricingRules.Enabled = true;
                    }
                    else
                    {
                        btnShowPricingRules.Enabled = false;
                    }
                    if (result.Components != null && result.Components.Count > 0)
                    {
                        //btnShowComponents.Enabled = true;
                    }
                    else
                    {
                        //btnShowComponents.Enabled = false;
                    }
                }
            };
        }

        private void btnShowPricingRules_Click(object sender, EventArgs e)
        {
            
                int? index = dataGridView1.CurrentRow?.Index;
                if (index.HasValue && index >= 0 && index < _results.Count)
                {
                    using (CustomerPriceRuleView customerPriceRuleView = new CustomerPriceRuleView(_results[index.Value].PricingRules))
                    {
                        customerPriceRuleView.ShowDialog(this);
                    }
                }
            
        }
    }
}
