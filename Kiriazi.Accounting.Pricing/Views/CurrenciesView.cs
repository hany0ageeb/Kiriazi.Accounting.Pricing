using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.ViewModels;
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
    public partial class CurrenciesView : Form
    {
        private readonly CurrencyController currencyController;
        private BindingList<CurrencyEditViewModel> _currencies;
       

        public CurrenciesView(CurrencyController currencyController)
        {
            InitializeComponent();
            this.currencyController = currencyController;
            
            Initialize();
        }
        private void Initialize()
        {
            _currencies = new BindingList<CurrencyEditViewModel>();
            Search();
            currencyGrid.AutoGenerateColumns = false;
            currencyGrid.AllowUserToAddRows = false;
            currencyGrid.AllowUserToDeleteRows = false;
            currencyGrid.MultiSelect = false;
            currencyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            currencyGrid.ReadOnly = true;
            currencyGrid.Columns.Clear();
            currencyGrid.Columns.AddRange(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Code",
                DataPropertyName = "Code",
                Name = "Code"
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "Name",
                DataPropertyName = "Name",
                Name = "Name"
            },
            new DataGridViewTextBoxColumn()
            {
                HeaderText = "Description",
                DataPropertyName = "Description",
                Name = "Description"
            },
            new DataGridViewCheckBoxColumn()
            {
                HeaderText = "Enabled",
                DataPropertyName = "IsEnabled",
                Name = "IsEnabled"
            },
            new DataGridViewButtonColumn()
            {
                HeaderText = "|",
                UseColumnTextForButtonValue = true,
                Name = "Edit",
                Text = "Edit"
            },
            new DataGridViewButtonColumn()
            {
                HeaderText = "|",
                UseColumnTextForButtonValue = true,
                Name = "Delete",
                Text = "Delete"
            });
            currencyGrid.CellContentClick += CurrencyGrid_CellContentClick;
            currencyGrid.DataBindings.Clear();
            currencyGrid.DataSource = _currencies;
           
        }

        private void CurrencyGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (grid != null) 
            {
                if (e.ColumnIndex == grid.Columns["Edit"].Index)
                {
                    using (CurrencyEditView currencyEditView = Program.ServiceProvider.GetRequiredService<CurrencyEditView>())
                    {
                        currencyEditView.ShowDialog(this);
                    }
                }
                else if (e.ColumnIndex == grid.Columns["Delete"].Index)
                {
                    Delete(e.RowIndex);
                }
            }
        }
        private void Search()
        {
            var currencies =  currencyController.Find();
            _currencies.Clear();
            foreach (var cur in currencies)
                _currencies.Add(cur);
        }
        private void Delete(int index)
        {
            DialogResult result = MessageBox.Show(this,
                                                          $"Are you sure you want to delete currency: {_currencies[index].Name} ?",
                                                          "Confirm",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string message = currencyController.DeleteCurrecny(_currencies[index].Id);
                _ = MessageBox.Show(this, message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Search();
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNewCurrency_Click(object sender, EventArgs e)
        {
            using(CurrencyEditView currencyEditView = new CurrencyEditView(new CurrencyEditViewModel(), currencyController))
            {
                DialogResult result = currencyEditView.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    
                }
                Search();
            }
        }
    }
}
