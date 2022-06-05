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
    public partial class ItemTreesView : Form
    {
        private readonly Controllers.ItemRelationController _controller;
        private ViewModels.ItemTreeSearchViewModel _model;
        private BindingList<ViewModels.ItemTreeViewModel> _trees = new BindingList<ViewModels.ItemTreeViewModel>();
        public ItemTreesView(Controllers.ItemRelationController controller)
        {
            _controller = controller;
            _model = _controller.Find();
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            cboCompanies.DataBindings.Clear();
            cboCompanies.DataSource = _model.Companies;
            cboCompanies.DisplayMember = nameof(Models.Company.Name);
            cboCompanies.ValueMember = nameof(Models.Company.Self);
            cboCompanies.DataBindings.Add(new Binding(nameof(cboCompanies.SelectedItem),_model,nameof(_model.Company)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged });
            //
            cboItems.DataBindings.Clear();
            cboItems.DataSource = _model.Items;
            cboItems.DisplayMember = nameof(Models.Item.Code);
            cboItems.ValueMember = nameof(Models.Item.Self);
            cboItems.DataBindings.Add(
                new Binding(nameof(cboItems.SelectedItem), _model, nameof(_model.Item)) 
                { 
                    DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged 
                });
            //
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.Columns.Clear();

            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemTreeViewModel.ItemCode),
                    DataPropertyName = nameof(ViewModels.ItemTreeViewModel.ItemCode),
                    ReadOnly = true,
                    HeaderText = "Code",
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemTreeViewModel.ItemArabicName),
                    DataPropertyName = nameof(ViewModels.ItemTreeViewModel.ItemArabicName),
                    ReadOnly = true,
                    HeaderText = "Name"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemTreeViewModel.ItemUomCode),
                    DataPropertyName = nameof(ViewModels.ItemTreeViewModel.ItemUomCode),
                    ReadOnly = true,
                    HeaderText = "Uom"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemTreeViewModel.CompanyName),
                    DataPropertyName = nameof(ViewModels.ItemTreeViewModel.CompanyName),
                    ReadOnly = true,
                    HeaderText = "Company"
                },
                new DataGridViewButtonColumn()
                {
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    HeaderText = ""
                },
                new DataGridViewButtonColumn()
                {
                    HeaderText = "",
                    UseColumnTextForButtonValue = true,
                    Text = "Delete"
                });
            dataGridView1.DataSource = _trees;
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
        private void Search()
        {
            _trees.Clear();
            var trees = _controller.Find(_model);
            foreach(var tree in trees)
            {
                _trees.Add(tree);
            }
            if(_trees.Count > 0)
            {
                btnNewFromExisting.Enabled = true;
            }
            else
            {
                btnNewFromExisting.Enabled = false;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            var companies = _controller.GetCompanies();
            if (companies.Count > 0)
            {
                using (CompanyItemRelationSelectorView companySelectorView = new CompanyItemRelationSelectorView(companies))
                {
                    companySelectorView.ShowDialog(this);
                    if(companySelectorView.DialogResult == DialogResult.OK)
                    {
                        var model = _controller.Add(companySelectorView.SelectedCompanies.Select(sc => sc.CompanyId).ToList());
                        using(ItemRelationEditView relationEditView = new ItemRelationEditView(model, _controller))
                        {
                            relationEditView.ShowDialog(this);
                        }
                    }
                }
            }
            else
            {
                _ = MessageBox.Show(this, "Define at least one company before creating any production tree.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
