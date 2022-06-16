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
    public partial class ItemsView : Form
    {
        private readonly Controllers.ItemController _itemController;
        private ViewModels.ItemSearchViewModel _model;
        private BindingList<Models.Item> _items = new BindingList<Models.Item>();

        public ItemsView(Controllers.ItemController itemController)
        {
            _itemController = itemController;
            _model = _itemController.Find();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            itemsGrid.AllowUserToAddRows = false;
            itemsGrid.AutoGenerateColumns = false;
            itemsGrid.AllowUserToDeleteRows = false;
            itemsGrid.ReadOnly = true;
            itemsGrid.MultiSelect = false;
            itemsGrid.Columns.Clear();
            itemsGrid.Columns.AddRange(new DataGridViewTextBoxColumn()
            {
                Name = "Code",
                DataPropertyName = "Code",
                HeaderText = "Code",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "Arabic Name",
                DataPropertyName = "ArabicName",
                HeaderText = "Arabic Name",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "English Name",
                DataPropertyName = "English Name",
                HeaderText = "English Name",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "AliasName",
                HeaderText = "Alias",
                DataPropertyName = "Alias",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "ItemTypeName",
                HeaderText = "Item Type",
                DataPropertyName = "ItemTypeName",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "UomName",
                HeaderText = "Uom",
                DataPropertyName = "UomName",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "TarrifCode",
                DataPropertyName = "TarrifCode",
                HeaderText = "Tarrif Code",
                ReadOnly = true
            },
            new DataGridViewTextBoxColumn()
            {
                Name = "TarrifPercentage",
                DataPropertyName = "TarrifPercentage",
                ReadOnly = true,
                HeaderText = "Tarrif(%)"
            },
            new DataGridViewButtonColumn()
            {
                HeaderText="",
                Text = "Edit",
                Name = "Edit",
                ReadOnly = true,
                UseColumnTextForButtonValue= true
            },
            new DataGridViewButtonColumn()
            {
                HeaderText = "",
                Text = "Delete",
                Name = "Delete",
                ReadOnly = true,
                UseColumnTextForButtonValue = true
            });
            itemsGrid.CellContentClick += (o, e) =>
            {
                DataGridView grid = o as DataGridView;
                if (e.RowIndex >= 0 && e.RowIndex < _items.Count)
                {
                    if(e.ColumnIndex == grid.Columns["Edit"].Index)
                    {
                        using (ItemEditView itemEditView = new ItemEditView(_itemController.Edit(_items[e.RowIndex].Id),_itemController))
                        {
                            itemEditView.ShowDialog(this);
                            Search();
                        }
                    }
                    else if(e.ColumnIndex == grid.Columns["Delete"].Index)
                    {
                        DialogResult result = MessageBox.Show(
                            this, 
                            $"Are you sure you want to delete item: {_items[e.RowIndex].Code} ?",
                            "Confirm delete",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if(result == DialogResult.Yes)
                        {
                            string message = _itemController.Delete(_items[e.RowIndex].Id);
                            Search();
                        }
                    }
                }
            };
            itemsGrid.DataSource = _items;
            //
            txtCode.DataBindings.Clear();
            txtCode.DataBindings.Add(new Binding(nameof(txtCode.Text),_model,nameof(_model.Code)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            txtArabicName.DataBindings.Clear();
            txtArabicName.DataBindings.Add(new Binding(nameof(txtArabicName.Text),_model,nameof(_model.ArabicName)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            txtEnglishName.DataBindings.Clear();
            txtEnglishName.DataBindings.Add(new Binding(nameof(txtEnglishName.Text),_model,nameof(_model.EnglishName)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            txtAlias.DataBindings.Clear();
            txtAlias.DataBindings.Add(new Binding(nameof(txtAlias.Text), _model,nameof(_model.NameAlias)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            cboItemTypes.DataBindings.Clear();
            cboItemTypes.DataBindings.Add(new Binding(nameof(cboItemTypes.SelectedItem), _model, nameof(_model.ItemType)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            cboItemTypes.DisplayMember = "Name";
            cboItemTypes.ValueMember = "Self";
            cboItemTypes.DataSource = _model.ItemTypes;
            //
           // cboCompanies.DataBindings.Clear();
           // cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_model,nameof(_model.Company)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
           // cboCompanies.DataSource = _model.Companies;
           // cboCompanies.DisplayMember = "Name";
           // cboCompanies.ValueMember = "Self";
        }
        private void Search()
        {
            _items.Clear();
            var items = _itemController.Find(_model);
            foreach(var item in items)
            {
                _items.Add(item);
            }
            if (_items.Count > 0)
            {
                btnCompanies.Enabled = true;
                btnCustomers.Enabled = true;
            }
            else
            {
                btnCompanies.Enabled = false;
                btnCustomers.Enabled = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            using (ItemEditView itemEditView = new ItemEditView(_itemController.Add(),_itemController))
            {
                itemEditView.ShowDialog(this);
                Search();
            }
        }
        private void btnCompanies_Click(object sender, EventArgs e)
        {
            if (itemsGrid.CurrentRow != null)
            {
                if(itemsGrid.CurrentRow.Index>=0 && itemsGrid.CurrentRow.Index < _items.Count)
                {
                    using(ItemCompaniesAssignmentView companiesAssignmentView = new ItemCompaniesAssignmentView(_itemController, _items[itemsGrid.CurrentRow.Index]))
                    {
                        companiesAssignmentView.ShowDialog(this);
                    }
                }
            }
        }
        private void btnCustomers_Click(object sender, EventArgs e)
        {
            int? index = itemsGrid.CurrentRow?.Index;
            if(index!=null && index >= 0 && index < _items.Count)
            {
                using(ItemCustomersAssignmentView itemCustomersAssignmentView = new ItemCustomersAssignmentView(_itemController, _items[index.Value]))
                {
                    itemCustomersAssignmentView.ShowDialog(this);
                }
            }
        }
    }
}
