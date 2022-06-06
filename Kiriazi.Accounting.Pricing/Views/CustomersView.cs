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
        }
        private void Search()
        {
            _customers.Clear();
            var customers = _controller.Find(txtName.Text);
            foreach(var customer in customers)
            {
                _customers.Add(customer);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
    }
}
