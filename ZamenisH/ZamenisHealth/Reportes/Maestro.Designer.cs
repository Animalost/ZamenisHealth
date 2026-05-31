namespace ZamenisHealth.Reportes
{
    partial class Maestro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Maestro));
            this.Universal = new Microsoft.Reporting.WinForms.ReportViewer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Universal
            // 
            this.Universal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Universal.Location = new System.Drawing.Point(0, 0);
            this.Universal.Name = "Universal";
            this.Universal.ServerReport.BearerToken = null;
            this.Universal.Size = new System.Drawing.Size(939, 723);
            this.Universal.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.Universal);
            this.panel1.Location = new System.Drawing.Point(-1, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(939, 723);
            this.panel1.TabIndex = 105;
            // 
            // Maestro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(938, 774);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Maestro";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Maestro_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public Microsoft.Reporting.WinForms.ReportViewer Universal;
        private System.Windows.Forms.Panel panel1;

        #endregion

        // private Microsoft.Reporting.WinForms.ReportViewer Universal;
    }
}