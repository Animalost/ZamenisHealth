namespace ZamenisHealth.Comunes
{
    partial class Login2
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
            this.patron1 = new FormAndControls.Controles.Patron();
            this.SuspendLayout();
            // 
            // patron1
            // 
            this.patron1.BackColor = System.Drawing.Color.White;
            this.patron1.Location = new System.Drawing.Point(12, 57);
            this.patron1.Name = "patron1";
            this.patron1.PatronMarcado = null;
            this.patron1.Size = new System.Drawing.Size(388, 381);
            this.patron1.TabIndex = 1;
            this.patron1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.patron1_MouseUp);
            // 
            // Login2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 450);
            this.Controls.Add(this.patron1);
            this.Name = "Login2";
            this.Text = "Login2";
            this.Load += new System.EventHandler(this.Login2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private FormAndControls.Controles.Patron patron1;
    }
}