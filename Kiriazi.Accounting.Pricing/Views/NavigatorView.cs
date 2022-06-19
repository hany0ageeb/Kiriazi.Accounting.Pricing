using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiriazi.Accounting.Pricing.Models;
using Microsoft.Extensions.Configuration;

namespace Kiriazi.Accounting.Pricing.Views
{
    public partial class NavigatorView : Form
    {
        public NavigatorView()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            
            var commands = Program.ServiceProvider.GetRequiredService<Controllers.UserCommandController>().Find();
            lstOptions.DataSource = commands;
            lstOptions.DisplayMember = "DisplayName";
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            ProcessUserSelection();
        }
        private void ProcessUserSelection()
        {
            UserCommand userCommand = lstOptions.SelectedItem as UserCommand;
            Form form = (Form)Program.ServiceProvider.GetRequiredService(Type.GetType(userCommand.FormType));
            form.MdiParent = this.MdiParent;
            form.Show();
        }
        private void lstOptions_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
                ProcessUserSelection();
        }
        private void lstOptions_DoubleClick(object sender, EventArgs e)
        {
            ProcessUserSelection();
        }
    }
   
}
