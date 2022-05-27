using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;
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
    public partial class CompaniesView : Form
    {
        private readonly CompanyController _controller;
        private BindingList<Company> _companies = new BindingList<Company>();
        public CompaniesView(CompanyController controller)
        {
            _controller = controller;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            companiesGrid.AutoGenerateColumns = false;
            companiesGrid.AllowUserToAddRows = false;
            companiesGrid.AllowUserToDeleteRows = false;
            companiesGrid.MultiSelect = false;
            companiesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            companiesGrid.Columns.Clear();
            companiesGrid.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Name",
                    DataPropertyName = "Name",
                    Name = "Name",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Name = "Description",
                    ReadOnly = true
                },
                new DataGridViewCheckBoxColumn()
                {
                    HeaderText = "Enabled",
                    DataPropertyName = "IsEnabled",
                    Name = "Description",
                    ReadOnly = true
                },
                new DataGridViewButtonColumn()
                {
                    HeaderText = "|",
                    Name = "Edit",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true
                },
                new DataGridViewButtonColumn()
                {
                    HeaderText = "|",
                    Name = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true
                });
            companiesGrid.DataSource = _companies;
            companiesGrid.CellContentClick += (o, e) =>
            {
                DataGridView gridView = o as DataGridView;
                if(e.ColumnIndex == gridView.Columns["Edit"].Index)
                {
                    Company company = _controller.Find(_companies[e.RowIndex].Id);
                    if (company != null)
                    {
                        using (CompanyEditView companyEditView = new CompanyEditView(_controller, company))
                        {
                            companyEditView.ShowDialog(this);
                        }
                        Search();
                    }
                }
                else if(e.ColumnIndex == gridView.Columns["Delete"].Index)
                {
                    DialogResult result = MessageBox.Show(this, $"Are you sure you want to delete Company: {_companies[e.RowIndex].Name}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string message = _controller.Delete(_companies[e.RowIndex].Id);
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
            };
            Search();
        }
        private void Search()
        {
            _companies.Clear();
            var comanies = _controller.Find();
            foreach(var comp in comanies)
            {
                _companies.Add(comp);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
