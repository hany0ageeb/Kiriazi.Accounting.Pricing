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
    public partial class ItemRelationEditView : Form
    {
        private readonly Controllers.ItemRelationController _controller;
        private readonly ViewModels.ItemRelationEditViewModel _model;
        private BindingList<ViewModels.ComponentViewModel> _lines = new BindingList<ViewModels.ComponentViewModel>();
        private AutoCompleteStringCollection _autoCompleteSource = new AutoCompleteStringCollection();
        private bool _hasChanged = false;

        public ItemRelationEditView(ViewModels.ItemRelationEditViewModel model,Controllers.ItemRelationController controller)
        {
            _controller = controller;
            _model = model;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            cboItems.DataBindings.Clear();
            cboItems.DataSource = _model.Items;
            cboItems.DisplayMember = nameof(Item.Code);
            cboItems.ValueMember = nameof(Item.Self);
            cboItems.DataBindings.Add(new Binding(nameof(cboItems.SelectedItem),_model,nameof(_model.ParentItem))
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });
            cboItems.SelectedIndexChanged += (o, e) =>
            {
                Item item = cboItems.SelectedItem as Item;
                _autoCompleteSource.Clear();
                if (item != null)
                {
                    var codes = _controller.FindItemsCodes(_model.CompaniesIds, item.Id);
                    _autoCompleteSource.AddRange(codes.ToArray());
                }
            };
            //
            txtItemName.DataBindings.Clear();
            txtItemName.DataBindings.Add(new Binding(nameof(txtItemName.Text), _model, nameof(_model.ItemName)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});
            //
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ComponentViewModel.ItemCode),
                    DataPropertyName = nameof(ViewModels.ComponentViewModel.ItemCode),
                    ReadOnly = false,
                    HeaderText = "Item"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ComponentViewModel.Quantity),
                    DataPropertyName = nameof(ViewModels.ComponentViewModel.Quantity),
                    ReadOnly = false,
                    HeaderText = "Quantity"
                });
            _lines = new BindingList<ViewModels.ComponentViewModel>(_model.Components);
            dataGridView1.DataSource = _lines;
            _autoCompleteSource.AddRange(_model.ItemCodes.ToArray());
            dataGridView1.EditingControlShowing += (o, e) =>
            {
                if(dataGridView1?.CurrentCell?.ColumnIndex == dataGridView1.Columns["ItemCode"].Index)
                {
                    TextBox control = e.Control as TextBox;
                    control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    control.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    control.AutoCompleteCustomSource = _autoCompleteSource;
                }
            };
            dataGridView1.CellValidating += (o, e) =>
            {
                if(e.ColumnIndex == dataGridView1.Columns["ItemCode"].Index)
                {
                    string itemCode = e.FormattedValue as string;
                    if (!_autoCompleteSource.Contains(itemCode))
                    {
                        e.Cancel = true;
                        System.Media.SystemSounds.Hand.Play();
                        dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Item code.";
                    }
                }
            };
            dataGridView1.CellValidated += (o, e) =>
            {
                dataGridView1.Rows[e.RowIndex].ErrorText = "";
                if (e.ColumnIndex == dataGridView1.Columns["ItemCode"].Index) 
                {
                    Item item = _controller.FindItemByCode(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string);
                    if (item != null)
                    {
                        txtComonentName.Text = item.ArabicName;
                        txtComponentUom.Text = item.UomName;
                    }
                    else
                    {
                        txtComonentName.Text = "";
                        txtComponentUom.Text = "";
                    }
                }
            };
            dataGridView1.RowValidating += (o, e) =>
            {
                if (_lines[e.RowIndex].Quantity <= 0)
                {
                    e.Cancel = true;
                    System.Media.SystemSounds.Hand.Play();
                    dataGridView1.Rows[e.RowIndex].ErrorText = "Invalid Quantity";
                }
                else
                {

                }
            };
            dataGridView1.RowValidated += (o, e) =>
            {
                dataGridView1.Rows[e.RowIndex].ErrorText = "";
            };
            dataGridView1.SelectionChanged += (o, e) =>
            {
                int? index = dataGridView1.CurrentRow?.Index;
                if(index!=null && index>=0 && index < _lines.Count)
                {
                    Item item = _controller.FindItemByCode(_lines[index.Value].ItemCode);
                    if (item != null)
                    {
                        txtComonentName.Text = item.ArabicName;
                        txtComponentUom.Text = item.UomName;
                    }
                    else
                    {
                        txtComonentName.Text = "";
                        txtComponentUom.Text = "";
                    }
                }
            };
            _model.PropertyChanged += (o, e) =>
            {
                if (_lines.Count > 0)
                {
                    _hasChanged = true;
                    btnSave.Enabled = true;
                }
            };
            _lines.ListChanged += (o, e) =>
            {
                if (_lines.Count > 0)
                {
                    _hasChanged = true;
                    btnSave.Enabled = true;
                }
            };
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _controller.Add(_model);
                if (modelState.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(string err in modelState.GetErrors())
                    {
                        sb.AppendLine(err);
                    }
                    for(int i= 0; i < modelState.InnerModelStatesCount; i++)
                    {
                        foreach(var err in modelState.GetModelState(i).GetErrors())
                        {
                            sb.AppendLine(err);
                        }
                    }
                    MessageBox.Show(this, sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    _hasChanged = false;
                    return true;
                }
            }
            return true;
        }
        private bool CloseForm()
        {
            if (_hasChanged)
            {
                DialogResult result = MessageBox.Show(
                    owner: this, 
                    text: "Do you want to save changes?", 
                    caption: "Confirm Save", 
                    buttons: MessageBoxButtons.YesNoCancel, 
                    icon: MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (SaveChanges())
                    {
                        _hasChanged = false;
                        return true;
                    }
                }
                else if(result == DialogResult.No)
                {
                    _hasChanged = false;
                    return true;
                }
            }
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                btnSave.Enabled = false;
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseForm())
                Close();
        }

        private void ItemRelationEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_hasChanged && (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.MdiFormClosing))
            {
                e.Cancel = !CloseForm();
            }
        }
    }
}
