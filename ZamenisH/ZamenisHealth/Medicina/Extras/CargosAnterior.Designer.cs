namespace ZamenisHealth.Medicina.Extras
{
    partial class CargosAnterior
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.gridZH1 = new ZamenisHealth.Clases.Controles.GridZH();
            this.gridZH2 = new ZamenisHealth.Clases.Controles.GridZH();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(10, 363);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(783, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "Cargos por Atencion";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Location = new System.Drawing.Point(6, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(655, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "Ultimas 20 Atenciones";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(5, 59);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 22);
            this.label14.TabIndex = 10;
            this.label14.Text = "label14";
            // 
            // gridZH1
            // 
            this.gridZH1.BackColor = System.Drawing.Color.White;
            this.gridZH1.Location = new System.Drawing.Point(5, 113);
            this.gridZH1.Name = "gridZH1";
            this.gridZH1.Size = new System.Drawing.Size(788, 247);
            this.gridZH1.TabIndex = 16;
            // 
            // gridZH2
            // 
            this.gridZH2.BackColor = System.Drawing.Color.White;
            this.gridZH2.Location = new System.Drawing.Point(5, 386);
            this.gridZH2.Name = "gridZH2";
            this.gridZH2.Size = new System.Drawing.Size(788, 280);
            this.gridZH2.TabIndex = 17;
            // 
            // CargosAnterior
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(805, 675);
            this.Controls.Add(this.gridZH2);
            this.Controls.Add(this.gridZH1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label14);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CargosAnterior";
            this.Load += new System.EventHandler(this.CargosAnterior_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label14;
        private Clases.Controles.GridZH gridZH1;
        private Clases.Controles.GridZH gridZH2;
    }
}