namespace ZamenisHealth.Recepcion.AgendaDiaria
{
    partial class RecibosAsociados
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
            this.gridZH1 = new ZamenisHealth.Clases.Controles.GridZH();
            this.SuspendLayout();
            // 
            // gridZH1
            // 
            this.gridZH1.BackColor = System.Drawing.Color.White;
            this.gridZH1.Location = new System.Drawing.Point(12, 64);
            this.gridZH1.Name = "gridZH1";
            this.gridZH1.Size = new System.Drawing.Size(776, 374);
            this.gridZH1.TabIndex = 1;
            // 
            // RecibosAsociados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gridZH1);
            this.Name = "RecibosAsociados";
            this.Text = "RecibosAsociados";
            this.Load += new System.EventHandler(this.RecibosAsociados_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Clases.Controles.GridZH gridZH1;
    }
}