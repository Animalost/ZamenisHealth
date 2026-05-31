namespace ZamenisHealth.Mensajeria
{
    partial class MainMessage
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnZamenis1 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis2 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis4 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(7, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(842, 415);
            this.panel1.TabIndex = 14;
            // 
            // btnZamenis1
            // 
            this.btnZamenis1.LabelText = "";
            this.btnZamenis1.Location = new System.Drawing.Point(7, 45);
            this.btnZamenis1.Name = "btnZamenis1";
            this.btnZamenis1.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis1.TabIndex = 15;
            // 
            // btnZamenis2
            // 
            this.btnZamenis2.LabelText = "";
            this.btnZamenis2.Location = new System.Drawing.Point(156, 45);
            this.btnZamenis2.Name = "btnZamenis2";
            this.btnZamenis2.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis2.TabIndex = 16;
            // 
            // btnZamenis4
            // 
            this.btnZamenis4.LabelText = "";
            this.btnZamenis4.Location = new System.Drawing.Point(306, 45);
            this.btnZamenis4.Name = "btnZamenis4";
            this.btnZamenis4.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis4.TabIndex = 18;
            // 
            // MainMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(857, 502);
            this.Controls.Add(this.btnZamenis4);
            this.Controls.Add(this.btnZamenis2);
            this.Controls.Add(this.btnZamenis1);
            this.Controls.Add(this.panel1);
            this.Name = "MainMessage";
            this.Load += new System.EventHandler(this.MainMessage_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private ControlUser.Controles.btnZamenis btnZamenis1;
        private ControlUser.Controles.btnZamenis btnZamenis2;
        private ControlUser.Controles.btnZamenis btnZamenis4;
    }
}