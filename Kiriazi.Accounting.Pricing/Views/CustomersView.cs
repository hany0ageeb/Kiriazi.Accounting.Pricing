using Kiriazi.Accounting.Pricing.Controllers;
using Kiriazi.Accounting.Pricing.Models;
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
    public partial class CustomersView : Form
    {
        private readonly CustomerController _controller;
        private BindingList<Customer> _customers = new BindingList<Customer>();

        public CustomersView(CustomerController customerController)
        {
            _controller = customerController;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Name",
                    DataPropertyName = nameof(Customer.Name),
                    Name = nameof(Customer.Name),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Description",
                    DataPropertyName = nameof(Customer.Description),
                    Name = nameof(Customer.Description),
                    ReadOnly = true
                },
                new DataGridViewButtonColumn()
                {
                    HeaderText = "",
                    Name = "Edit",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    ReadOnly = true
                },
                new DataGridViewButtonColumn()
                {
                    HeaderText = "",
                    Name = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    ReadOnly = true
                });
            dataGridView1.DataSource = _customers;
            Search();
            dataGridView1.CellContentClick += (o, e) =>
            {
                if(e.ColumnIndex == dataGridView1.Columns["Edit"].Index)
                {
                    if (e.RowIndex >= 0 && e.RowIndex < _customers.Count) 
                    {
                        using (CustomerEditView customerEditView = new CustomerEditView(_controller, _controller.Edit(_customers[e.RowIndex].Id)))
                        {
                            _ = customerEditView.ShowDialog(this);
                            if (_customers.Count > 0)
                                Search();
                        }
                    }
                }
                else if(e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
                {
                    if (e.RowIndex >= 0 && e.RowIndex < _customers.Count)
                    {
                        DialogResult result = MessageBox.Show(this, $"Are you sure you want to delete Customer: {_customers[e.RowIndex].Name}", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string message = _controller.Delete(_customers[e.RowIndex].Id);
                            if (!string.IsNullOrEmpty(message))
                            {
                                _ = MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if(_customers.Count > 0)
                                    Search();
                            }
                        }
                    }
                }
            };
        }
        private void Search()
        {
            _customers.Clear();
            var customers = _controller.Find(txtName.Text);
            foreach(var customer in customers)
            {
                _customers.Add(customer);
            }
            if (_customers.Count > 0)
            {
                //btnPricingRules.Enabled = true;
            }
            else
            {
                //btnPricingRules.Enabled = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using(CustomerEditView customerEditView = Program.ServiceProvider.GetRequiredService<CustomerEditView>())
            {
                customerEditView.ShowDialog(this);
                if(_customers.Count > 0)
                    Search();
            }
        }
        /*
        private void btnPricingRules_Click(object sender, EventArgs e)
        {
            int? index = dataGridView1.CurrentRow?.Index;
            if(index != null && index >= 0 && index < _customers.Count)
            {
                var rules = _controller.EditCustomerPricingRules(_customers[index.Value].Id);
                using(CustomerPricingRuleEditView pricingRuleEditView  = new CustomerPricingRuleEditView(_controller,rules,_customers[index.Value]))
                {
                    _ = pricingRuleEditView.ShowDialog(this);
                }
            }
        }
        */
    }
}
