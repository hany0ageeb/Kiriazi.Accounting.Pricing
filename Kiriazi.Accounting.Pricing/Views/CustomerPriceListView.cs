using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiriazi.Accounting.Pricing.ViewModels;
using Npoi.Mapper;

namespace Kiriazi.Accounting.Pricing.Views
{
    public partial class CustomerPriceListsView : Form
    {
        private BindingList<CustomerPriceListViewModel> _lines;
        public CustomerPriceListsView(IList<CustomerPriceListViewModel> lines)
        {
            InitializeComponent();
            _lines = new BindingList<CustomerPriceListViewModel>(lines);
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;
            dataGridView1.ShowCellErrors = false;
            dataGridView1.ShowRowErrors = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.CompanyName),
                    Name = nameof(CustomerPriceListViewModel.CompanyName),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.CustomerName),
                    Name = nameof(CustomerPriceListViewModel.CustomerName),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn() 
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.PriceListDate),
                    Name = nameof(CustomerPriceListViewModel.PriceListDate)
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.ItemCode),
                    Name = nameof(CustomerPriceListViewModel.ItemCode)
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.ItemArabicName),
                    Name = nameof(CustomerPriceListViewModel.ItemArabicName)
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.ItemEnglishName),
                    Name = nameof(CustomerPriceListViewModel.ItemEnglishName)
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.ItemAlias),
                    Name = nameof(CustomerPriceListViewModel.ItemAlias)
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.UomCode),
                    Name = nameof(CustomerPriceListViewModel.UomCode)
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.CurrencyCode),
                    Name = nameof(CustomerPriceListViewModel.CurrencyCode)
                },
                new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nameof(CustomerPriceListViewModel.UnitPrice),
                    Name = nameof(CustomerPriceListViewModel.UnitPrice)
                });
            saveFileDialog1.Filter = "Excel Files | *.xlsx";
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.OverwritePrompt = true;
            dataGridView1.DataSource = _lines;
            dataGridView1.AutoResizeColumns();
            this.Enter += (o, e) =>
            {
                MainView main = this.MdiParent as MainView;
                main.ClearExportMenuItemClickEventHandlers();
                if(_lines.Count > 0)
                {
                    main.AddExportMenuItemClickEventHandler(ExportMenuItem_Click);
                    main.IsExportMenuItemEnabled = true;
                }
                else
                {
                    main.IsExportMenuItemEnabled = false;
                }
            };
            this.Leave += (o, e) =>
            {
                MainView main = this.MdiParent as MainView;
                main.ClearExportMenuItemClickEventHandlers();
                main.IsExportMenuItemEnabled = false;
            };
        }
        private void ExportMenuItem_Click(object sender,EventArgs args)
        {
            DialogResult result = saveFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                Mapper mapper = new Mapper();
                mapper.Save<CustomerPriceListViewModel>(saveFileDialog1.FileName,_lines.ToList());
            } 
        }
    }
}
