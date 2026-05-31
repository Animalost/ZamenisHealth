namespace ZamenisHealth.Medicina
{
    partial class Historial_Medico_3
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.VistaPrevia = new Microsoft.Reporting.WinForms.ReportViewer();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.VistaPrevia);
            this.panel2.Location = new System.Drawing.Point(142, 221);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(806, 559);
            this.panel2.TabIndex = 42;
            // 
            // VistaPrevia
            // 
            this.VistaPrevia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VistaPrevia.Location = new System.Drawing.Point(0, 0);
            this.VistaPrevia.Name = "VistaPrevia";
            this.VistaPrevia.ServerReport.BearerToken = null;
            this.VistaPrevia.ShowBackButton = false;
            this.VistaPrevia.ShowExportButton = false;
            this.VistaPrevia.ShowFindControls = false;
            this.VistaPrevia.ShowPageNavigationControls = false;
            this.VistaPrevia.ShowPrintButton = false;
            this.VistaPrevia.ShowRefreshButton = false;
            this.VistaPrevia.ShowStopButton = false;
            this.VistaPrevia.ShowToolBar = false;
            this.VistaPrevia.ShowZoomControl = false;
            this.VistaPrevia.Size = new System.Drawing.Size(806, 559);
            this.VistaPrevia.TabIndex = 16;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(142, 111);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(806, 106);
            this.textBox2.TabIndex = 39;
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "",
            "Deseo reimprimir una copia de la orden medica",
            "Deseo renovar la orden medica identica",
            "Deseo actualizar algunos datos y renovar la orden medica"});
            this.comboBox3.Location = new System.Drawing.Point(357, 76);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(591, 32);
            this.comboBox3.TabIndex = 38;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(142, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(203, 26);
            this.label6.TabIndex = 36;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(139, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(206, 17);
            this.label5.TabIndex = 35;
            this.label5.Text = "Numero de orden seleccionado";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(354, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 17);
            this.label1.TabIndex = 43;
            this.label1.Text = "Accion";
            // 
            // Historial_Medico_3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(955, 788);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Historial_Medico_3";
            this.Load += new System.EventHandler(this.Historial_Medico_3_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private Microsoft.Reporting.WinForms.ReportViewer VistaPrevia;
        private System.Windows.Forms.TextBox textBox2;
        public System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
    }
}