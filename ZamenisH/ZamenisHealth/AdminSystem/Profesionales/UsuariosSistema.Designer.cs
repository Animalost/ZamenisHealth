namespace ZamenisHealth.AdminSystem.Profesionales
{
    partial class UsuariosSistema
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
            this.gridZH1.Location = new System.Drawing.Point(7, 56);
            this.gridZH1.Name = "gridZH1";
            this.gridZH1.Size = new System.Drawing.Size(696, 603);
            this.gridZH1.TabIndex = 3;
            // 
            // UsuariosSistema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 664);
            this.Controls.Add(this.gridZH1);
            this.Name = "UsuariosSistema";
            this.Text = "UsuariosSistema";
            this.Load += new System.EventHandler(this.UsuariosSistema_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Clases.Controles.GridZH gridZH1;
    }
}