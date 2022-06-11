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
    public partial class ItemTreeViewer : Form
    {
        private readonly Controllers.ItemRelationController _controller;
        private readonly IList<ViewModels.ItemRelationViewModel> _tree;
        private  IList<ViewModels.ItemRelationViewModel> _treeCopy;
        private BindingList<ViewModels.ItemRelationViewModel> _relations;
        private  Guid companyId;
        private  Guid rootId;
        public ItemTreeViewer(Controllers.ItemRelationController itemRelationController, IList<ViewModels.ItemRelationViewModel> tree)
        {
            _controller = itemRelationController;
            _tree = tree;
            InitializeComponent();
            InitializeDataGridColumns();
            Initialize();
        }
        private void InitializeDataGridColumns()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.AddRange(
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemRelationViewModel.ComponentCode),
                    DataPropertyName = nameof(ViewModels.ItemRelationViewModel.ComponentCode),
                    ReadOnly = true,
                    HeaderText = "Component Code"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemRelationViewModel.ComponentArabicName),
                    DataPropertyName = nameof(ViewModels.ItemRelationViewModel.ComponentArabicName),
                    HeaderText = "Component Name",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemRelationViewModel.ComponentUomCode),
                    DataPropertyName = nameof(ViewModels.ItemRelationViewModel.ComponentUomCode),
                    HeaderText = "Uom",
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = nameof(ViewModels.ItemRelationViewModel.ComponentQuantity),
                    DataPropertyName = nameof(ViewModels.ItemRelationViewModel.ComponentQuantity),
                    HeaderText = "Quantity",
                    ReadOnly = true
                });
        }
        private void Initialize()
        {
            if (_tree != null && _tree.Count > 0)
            {
                companyId = _tree[0].CompanyId;
                rootId = _tree[0].RootId;
                Text = $"Product {_tree[0].RootCode} / Tree Company {_tree[0].CompanyName}";
                treeView1.Nodes.Add(rootId.ToString(),_tree[0].RootCode);
                foreach (var r in _tree)
                {
                    treeView1.Nodes[0].Nodes.Add(r.ComponentId.ToString(), r.ComponentCode);
                }
            }
            _treeCopy = new List<ViewModels.ItemRelationViewModel>(_tree);
            _relations = new BindingList<ViewModels.ItemRelationViewModel>(_treeCopy);
            dataGridView1.DataSource = _relations;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(_tree != null && _tree.Count > 0)
            {
                if (e.Node.IsSelected )
                {
                    var temp = _controller.Find(companyId, Guid.Parse(e.Node.Name));
                    _relations.Clear();
                    e.Node.Nodes.Clear();
                    foreach (var c in temp)
                    {
                        e.Node.Nodes.Add(c.ComponentId.ToString(), c.ComponentCode);
                        _relations.Add(c);
                    }
                   
                }
            }
        }
    }
}
