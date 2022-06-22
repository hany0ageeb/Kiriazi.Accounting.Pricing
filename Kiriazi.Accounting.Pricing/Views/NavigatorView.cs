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
            List<UserCommandAssignment> commands = new List<UserCommandAssignment>();
            if (Common.Session.CurrentUser != null)
            {
                Text = "Navigator - " + Common.Session.CurrentUser.UserName;
                commands.AddRange(Common.Session.CurrentUser.UserCommands.Where(ass=>ass.Command.CommandType==UserCommandType.NormalCommand).OrderBy(ass=>ass.Sequence).ThenBy(ass=>ass.DisplayName));
               
            }
            lstOptions.DataSource = commands;
            lstOptions.DisplayMember = "DisplayName";
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                UserReport report = Common.Session.CurrentUser.UserReports.Where(ass => ass.Report.Name == menuItem.Name).Select(ass => ass.Report).FirstOrDefault();
                if (report != null)
                {
                    if (!string.IsNullOrEmpty(report.ParameterFormTypeName))
                    {
                        Form parameterFrom = (Form)Program.ServiceProvider.GetService(Type.GetType(report.ParameterFormTypeName));
                        parameterFrom.MdiParent = this.MdiParent;
                        parameterFrom.Show();
                    }
                    else if (!string.IsNullOrEmpty(report.ReportFormTypeName))
                    {
                        Form parameterFrom = (Form)Program.ServiceProvider.GetService(Type.GetType(report.ReportFormTypeName));
                        parameterFrom.MdiParent = this.MdiParent;
                        parameterFrom.Show();
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            ProcessUserSelection();
        }
        private void ProcessUserSelection()
        {
            UserCommandAssignment userCommand = lstOptions.SelectedItem as UserCommandAssignment;
            if (userCommand != null)
            {
                Form form = (Form)Program.ServiceProvider.GetRequiredService(Type.GetType(userCommand.Command.FormType));
                form.MdiParent = this.MdiParent;
                form.Show();
            }
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

        private void NavigatorView_Load(object sender, EventArgs e)
        {

        }

        private void NavigatorView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
   
}
