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
    public partial class UomEditView : Form
    {
        private readonly Controllers.UomController _uomController;
        private Models.Uom _uom;
        private bool _hasChanged = false;

        public UomEditView(Controllers.UomController controller,Models.Uom uom)
        {
            _uomController = controller;
            _uom = uom;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            txtCode.DataBindings.Clear();
            txtCode.DataBindings.Add(new Binding(nameof(txtCode.Text), _uom, nameof(_uom.Code)));
            //
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add(new Binding(nameof(txtName.Text), _uom, nameof(_uom.Name)));
            //
            _uom.PropertyChanged += (o, e) =>
              {
                  _hasChanged = true;
                  btnSave.Enabled = true;
              };
        }
        private void txtCode_Validating(object sender, CancelEventArgs e)
        {
            TextBox control = sender as TextBox;
            if (string.IsNullOrEmpty(control.Text))
            {
                errorProvider1.SetError(control, "Unit of Measure Code Field is mandatory.");
                e.Cancel = true;
            }
            else if(control.Text.Length > 4)
            {
                errorProvider1.SetError(control, "Unit of Measure code should be no more than 4 letters.");
                e.Cancel = true;
            }
        }

        private void txtCode_Validated(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;
            errorProvider1.SetError(control, "");
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            TextBox control = sender as TextBox;
            if (string.IsNullOrEmpty(control.Text))
            {
                errorProvider1.SetError(control, "Unit of Measure Name Field is mandatory.");
                e.Cancel = true;
            }
        }

        private void txtName_Validated(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;
            errorProvider1.SetError(control, "");
        }
        private bool SaveChanges()
        {
            if (_hasChanged)
            {
                var modelState =  _uomController.SaveOrUpdate(_uom);
                if (modelState.HasErrors)
                {
                    var errors = modelState.GetErrors(nameof(_uom.Code));
                    if (errors != null && errors.Count > 0)
                    {
                        errorProvider1.SetError(txtCode, errors[0]);
                    }
                    errors = modelState.GetErrors(nameof(_uom.Name));
                    if (errors != null && errors.Count > 0)
                    {
                        errorProvider1.SetError(txtName, errors[0]);
                    }
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
                else
                {
                    errorProvider1.SetError(txtCode, "");
                    errorProvider1.SetError(txtName, "");
                    System.Media.SystemSounds.Beep.Play();
                    _hasChanged = false;
                    return true;
                }
            }
            return true;
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
                        Close();
                    }
                }
                else if(result == DialogResult.No)
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
            if (_hasChanged)
            {
                if (SaveChanges())
                {
                    btnSave.Enabled = false;
                    _hasChanged = false;
                    Close();
                }
            }
        }
    }
}
