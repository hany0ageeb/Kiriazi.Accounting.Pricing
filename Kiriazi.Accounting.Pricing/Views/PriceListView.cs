using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;
using Kiriazi.Accounting.Pricing.ViewModels;
using Npoi.Mapper;

namespace Kiriazi.Accounting.Pricing.Views
{
   
    public partial class PriceListView : Form
    {
       
        private PriceListViewModel _model;
        private BindingSource _bindingSource;
        private DataView _lines ;
        
        public PriceListView(PriceListViewModel model)
        {
            _model = model;
            InitializeComponent();
            Initialize();
        }
        private void ExportMenuItem_Click(object sender,EventArgs args)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    Mapper mapper = new Mapper();
                    mapper.Save<PriceListLineViewModel>(saveFileDialog1.FileName, _model.Lines.OrderBy(l=>l.ItemCode));
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
        private void Initialize()
        {
            saveFileDialog1.Filter = "Excel Files | *.xlsx";
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.OverwritePrompt = true;
            Enter += (o, e) =>
            {
                if (_model.Lines.Count > 0)
                {
                    MainView mainView = MdiParent as MainView;
                    if (mainView != null)
                    {
                        mainView.AddExportMenuItemClickEventHandler(ExportMenuItem_Click);
                        mainView.IsExportMenuItemEnabled = true;
                    }
                }
                else
                {
                    MainView mainView = MdiParent as MainView;
                    if (mainView != null)
                    {
                        mainView.ClearExportMenuItemClickEventHandlers();
                        mainView.IsExportMenuItemEnabled = false;
                    }
                }
            };
            Leave += (o, e) =>
            {
                MainView mainView = MdiParent as MainView;
                if (mainView != null)
                {
                    mainView.ClearExportMenuItemClickEventHandlers();
                    mainView.IsExportMenuItemEnabled = false;
                }
            };
            DataTable dataTable = new DataTable("PriceListLines");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn()
                {
                    ColumnName = "ItemCode",
                    AllowDBNull = false,
                    AutoIncrement = false,
                    DataType = typeof(string),
                    Caption = "Item"
                },
                new DataColumn()
                {
                    ColumnName = "ItemName",
                    AllowDBNull = false,
                    DataType = typeof(string),
                    Caption = "Name"
                },
                new DataColumn()
                {
                    ColumnName = "ItemUom",
                    AllowDBNull = false,
                    DataType = typeof(string),
                    Caption = "Uom"
                },
                new DataColumn()
                {
                    ColumnName = "UnitPrice",
                    AllowDBNull = false,
                    DataType = typeof(decimal),
                    Caption = "Unit Price"
                },
                new DataColumn()
                {
                    ColumnName = "CurrencyCode",
                    DataType = typeof(string),
                    AllowDBNull = false,
                    Caption = "Currency"
                },
                new DataColumn()
                {
                    ColumnName = "CurrencyExchangeRateType",
                    DataType = typeof(string),
                    AllowDBNull = true,
                    Caption = "Currency Exchange Rate Type"
                },
                new DataColumn()
                {
                    ColumnName = "CurrencyExchangeRate",
                    DataType = typeof(decimal),
                    AllowDBNull = true,
                    Caption = "Currency Exchange Rate"
                },
                new DataColumn()
                {
                    ColumnName = "TarrifType",
                    DataType = typeof(string),
                    AllowDBNull = true,
                    Caption = "Tarrif Type"
                },
                new DataColumn()
                {
                    ColumnName = "TarrifPercentage",
                    DataType = typeof(decimal),
                    AllowDBNull = true,
                    Caption = "Tarrif Percentage(%)"
                }
            });
            foreach (var line in _model.Lines.OrderBy(l=>l.ItemCode))
            {
                var row = dataTable.NewRow();
                row["ItemCode"] = line.ItemCode;
                row["ItemName"] = line.ItemName;
                row["ItemUom"] = line.ItemUom;
                if (line.TarrifPercentage == null)
                    row["TarrifPercentage"] = DBNull.Value;
                else
                    row["TarrifPercentage"] = line.TarrifPercentage;
                row["TarrifType"] = line.TarrifType;
                row["UnitPrice"] = line.UnitPrice;
                row["CurrencyCode"] = line.CurrencyCode;
                if (line.CurrencyExchangeRate == null)
                    row["CurrencyExchangeRate"] = DBNull.Value;
                else
                    row["CurrencyExchangeRate"] = line.CurrencyExchangeRate;

                row["CurrencyExchangeRateType"] = line.CurrencyExchangeRateType;
                dataTable.Rows.Add(row);
            }
            _lines = new DataView(dataTable);
            _lines.AllowDelete = false;
            _lines.AllowEdit = false;
            _bindingSource = new BindingSource() { DataSource = _lines };
            //
            cboItems.DataBindings.Clear();
            var distItemCode = _model.Lines.Select(l => l.ItemCode).Distinct().ToList();
            distItemCode.Insert(0, "");
            cboItems.DataSource = distItemCode;
            cboItems.SelectedIndexChanged += (o, e) =>
            {
                string code = cboItems.SelectedItem as string;
                if (string.IsNullOrEmpty(code))
                {
                    _bindingSource.RemoveFilter();
                }
                else
                {
                    if (_bindingSource.SupportsFiltering)
                    {
                        _bindingSource.Filter = "ItemCode = '" + code + "'";
                        _bindingSource.ResetBindings(false);
                    }
                }
                txtCount.Text = dataGridView1.Rows.Count.ToString();
            };
            //
            txtName.Text = _model.PriceListName;
            //
            
            //
            txtAccountingPeriodName.Text = _model.AccountingPeriodName;
            //
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    Name = "ItemCode",
                    DataPropertyName = "ItemCode",
                    ReadOnly = true,
                    HeaderText = "Item",
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "ItemName",
                    DataPropertyName = "ItemName",
                    ReadOnly = true,
                    HeaderText = "Name",
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "ItemUom",
                    DataPropertyName = "ItemUom",
                    HeaderText = "Uom",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "UnitPrice",
                    DataPropertyName = "UnitPrice",
                    HeaderText = "Unit Price",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "CurrencyCode",
                    DataPropertyName = "CurrencyCode",
                    HeaderText = "Currency",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "CurrencyExchangeRateType",
                    DataPropertyName = "CurrencyExchangeRateType",
                    ReadOnly = true,
                    HeaderText = "Currency Exchange Rate Type",
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "CurrencyExchangeRate",
                    DataPropertyName = "CurrencyExchangeRate",
                    ReadOnly = true,
                    HeaderText = "Currency Exchange Rate",
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "TarrifType",
                    DataPropertyName = "TarrifType",
                    HeaderText = "Tarrif Type",
                    ReadOnly = true,
                    Visible = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "TarrifPercentage",
                    DataPropertyName = "TarrifPercentage",
                    HeaderText = "Tarrif Percentage(%)",
                    ReadOnly = true,
                    Visible = true
                });
            dataGridView1.DataSource = _bindingSource;
            txtCount.Text = dataGridView1.Rows.Count.ToString();
            
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
           Close();
        }       
    }
}
