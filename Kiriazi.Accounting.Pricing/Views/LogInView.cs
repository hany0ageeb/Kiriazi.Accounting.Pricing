using Kiriazi.Accounting.Pricing.Controllers;
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
    public partial class LogInView : Form
    {
        private readonly UserController _userController;
        public LogInView(UserController userController)
        {
            _userController = userController;
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                _ = MessageBox.Show(this, "Invalid User Name");
                return;
            }
            Common.Session.CurrentUser = _userController.LogIn(txtUserName.Text,txtPassword.Text);
            if (Common.Session.CurrentUser != null)
            {
                Close();
            }
            else
            {
                _ = MessageBox.Show(this, "Invalid User Name / Password");
                return;
            }
        }
    }
}
