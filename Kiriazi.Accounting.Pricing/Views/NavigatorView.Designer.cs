
namespace Kiriazi.Accounting.Pricing.Views
{
    partial class NavigatorView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstOptions = new System.Windows.Forms.ListBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstOptions
            // 
            this.lstOptions.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstOptions.FormattingEnabled = true;
            this.lstOptions.ItemHeight = 17;
            this.lstOptions.Items.AddRange(new object[] {
            "عملة"});
            this.lstOptions.Location = new System.Drawing.Point(12, 12);
            this.lstOptions.Name = "lstOptions";
            this.lstOptions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lstOptions.Size = new System.Drawing.Size(424, 531);
            this.lstOptions.TabIndex = 0;
            this.lstOptions.DoubleClick += new System.EventHandler(this.lstOptions_DoubleClick);
            this.lstOptions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstOptions_KeyDown);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(332, 562);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(103, 37);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "فتح";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // NavigatorView
            // 
            this.AcceptButton = this.btnOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 604);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.lstOptions);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "NavigatorView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Navigator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstOptions;
        private System.Windows.Forms.Button btnOpen;
    }
}