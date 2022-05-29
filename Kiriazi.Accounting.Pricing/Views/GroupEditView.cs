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
    public partial class GroupEditView : Form
    {
        private readonly Controllers.GroupController _groupController;
        private Models.Group _group;
        private bool _hasChanged = false;

        public GroupEditView(Controllers.GroupController groupController,Models.Group group)
        {
            _groupController = groupController;
            _group = group;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text), _group, nameof(_group.Name)));
            txtName.Validating += (o, e) =>
            {
                TextBox control = o as TextBox;
                if (string.IsNullOrEmpty(control.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(control, "Please enter Group name.");

                }
            };
            txtName.Validated += (o, e) =>
            {
                TextBox control = o as TextBox;
                errorProvider1.SetError(control, "");
            };
            //
            txtDescription.DataBindings.Clear();
            txtDescription.DataBindings.Add(new Binding(nameof(txtDescription.Text), _group, nameof(_group.Description)) { DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged});
            //
            _group.PropertyChanged += (o, e) =>
            {
                _hasChanged = true;
                btnSave.Enabled = true;
            };
        }
        private bool saveChanges()
        {
            if (_hasChanged)
            {
                var modelState = _groupController.SaveOrUpdate(_group);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors(nameof(_group.Name));
                    if(errors.Count > 0)
                    {
                        errorProvider1.SetError(txtName, errors[0]);
                    }
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtName, "");
                    _hasChanged = false;
                    btnSave.Enabled = false;
                    System.Media.SystemSounds.Beep.Play();
                    return true;
                }
            }
            return true;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_hasChanged)
            {
                DialogResult result =
                    MessageBox.Show(
                        owner: this,
                        text: "Do you want to Save Changes?",
                        caption: "Confirm save",
                        buttons: MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (saveChanges())
                    {
                        Close();
                    }
                }
                else if (result == DialogResult.No)
                {
                    _hasChanged = false;
                    btnSave.Enabled = false;
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveChanges())
            {
                btnSave.Enabled = false;
                _hasChanged = false;
                Close();
            }
        }
    }
}
