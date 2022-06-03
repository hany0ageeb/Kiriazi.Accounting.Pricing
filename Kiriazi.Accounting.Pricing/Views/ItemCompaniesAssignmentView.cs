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
    public partial class ItemCompaniesAssignmentView : Form
    {
        private readonly Controllers.ItemController _itemController;
        private Models.Item _item;
        private BindingList<ViewModels.ItemCompanyAssignmentViewModel> assignments;
        private List<Models.Group> _groups = new List<Models.Group>();
        private bool _hasChanged = false;
        private bool _oldValue;
        public ItemCompaniesAssignmentView(Controllers.ItemController controller,Models.Item item)
        {
            _itemController = controller;
            _item = item;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            this.Text = $"Item {_item.Code} Companies Assignment";
            assignments = new BindingList<ViewModels.ItemCompanyAssignmentViewModel>(_itemController.EditItemCompanyAssignment(_item.Id));
            if (assignments.Count > 0)
                _groups.AddRange(assignments[0].Groups);
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
                    DataPropertyName = "IsAssigned",
                    ReadOnly = false
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Company",
                    Name = "CompanyName",
                    DataPropertyName = "CompanyName",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    HeaderText = "Item Name Alias",
                    Name = "Alise",
                    DataPropertyName = "Alise",
                    ReadOnly = false
                },
                new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Group",
                    ReadOnly = false,
                    Name = "Group",
                    DataPropertyName = "Group",
                    DataSource = _groups,
                    DisplayMember = "Name",
                    ValueMember = "Self"
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
                else if(e.ColumnIndex == dataGridView1.Columns["Group"].Index)
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
                            assignments[e.RowIndex].Group = _groups[0];
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
                    _itemController.EditItemCompanyAssignment(assignments, _item.Id);
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
