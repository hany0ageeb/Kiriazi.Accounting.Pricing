using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kiriazi.Accounting.Pricing.Views
{
    public partial class ItemCustomersAssignmentView : Form
    {
        private readonly Controllers.ItemController _itemController;
        private Item _item;
        private BindingList<ViewModels.ItemCustomerAssignmentViewModel> assignments;
        
        private bool _hasChanged = false;
        private bool _oldValue;
        public ItemCustomersAssignmentView(Controllers.ItemController controller,Item item)
        {
            _itemController = controller;
            _item = item;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            this.Text = $"Item {_item.Code} Customers Assignment";
            assignments = new BindingList<ViewModels.ItemCustomerAssignmentViewModel>(_itemController.EditItemCustomerAssignment(_item.Id));
            
            //
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.MultiSelect = false;

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewCheckBoxColumn()
                {
                    HeaderText = "Assigned",
                    Name = "IsAssigned",
                    DataPropertyName = nameof(ViewModels.ItemCustomerAssignmentViewModel.IsAssigned),
                    ReadOnly = false
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Customer",
                    Name = "CompanyName",
                    DataPropertyName = nameof(ViewModels.ItemCustomerAssignmentViewModel.CustomerName),
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Item Name Alias",
                    Name = "Alise",
                    DataPropertyName = nameof(ViewModels.ItemCustomerAssignmentViewModel.Alise),
                    ReadOnly = false
                });
            assignments.ListChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
            btnSave.Enabled = false;
            dataGridView1.DataSource = assignments;
            dataGridView1.CellBeginEdit += (o, e) =>
            {
                if (e.ColumnIndex == dataGridView1.Columns["Alise"].Index) 
                {
                    e.Cancel = !assignments[e.RowIndex].IsAssigned;
                }
                else if (e.ColumnIndex == dataGridView1.Columns["IsAssigned"].Index)
                {
                    _oldValue = assignments[e.RowIndex].IsAssigned;
                }
            };
            dataGridView1.CellEndEdit += (o, e) =>
            {
                if(e.ColumnIndex == dataGridView1.Columns["IsAssigned"].Index)
                {
                    if (assignments[e.RowIndex].IsAssigned != _oldValue)
                    {
                        if (assignments[e.RowIndex].IsAssigned)
                        {
                            if (string.IsNullOrEmpty(assignments[e.RowIndex].Alise))
                            {
                                assignments[e.RowIndex].Alise = _item.Alias ?? "";
                            }
                        }
                        else
                        {
                            assignments[e.RowIndex].Alise = "";
                        }
                    }
                }
            };
            dataGridView1.EditingControlShowing += (o, e) =>
            {
                ComboBox comboBox = e.Control as ComboBox;
                if (comboBox != null)
                {
                    comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                }
            };
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                try
                {
                    _itemController.EditItemCustomerAssignment(assignments, _item.Id);
                    return true;
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show(this,ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                _hasChanged = false;
                btnSave.Enabled = false;
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChanged = false;
                        Close();
                    }
                }
                else if (result == DialogResult.No)
                {
                    _hasChanged = false;
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void ItemCompaniesAssignmentView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hasChanged && (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.MdiFormClosing))
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes?", "Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    e.Cancel = !SaveChanges();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
