namespace ZamenisHealth.Medicina.OrdenesHistory
{
    partial class TipoOrdenMedica
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
            this.boton1 = new FormAndControls.Controles.Boton();
            this.boton2 = new FormAndControls.Controles.Boton();
            this.boton3 = new FormAndControls.Controles.Boton();
            this.boton4 = new FormAndControls.Controles.Boton();
            this.SuspendLayout();
            // 
            // boton1
            // 
            this.boton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton1.FlatAppearance.BorderSize = 3;
            this.boton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton1.Location = new System.Drawing.Point(35, 92);
            this.boton1.Name = "boton1";
            this.boton1.Size = new System.Drawing.Size(176, 51);
            this.boton1.TabIndex = 1;
            this.boton1.Text = "Orden de Servicios";
            this.boton1.UseVisualStyleBackColor = false;
            this.boton1.Click += new System.EventHandler(this.boton1_Click);
            // 
            // boton2
            // 
            this.boton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton2.FlatAppearance.BorderSize = 3;
            this.boton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton2.Location = new System.Drawing.Point(35, 163);
            this.boton2.Name = "boton2";
            this.boton2.Size = new System.Drawing.Size(176, 51);
            this.boton2.TabIndex = 2;
            this.boton2.Text = "Orden de Medicamentos";
            this.boton2.UseVisualStyleBackColor = false;
            this.boton2.Click += new System.EventHandler(this.boton2_Click);
            // 
            // boton3
            // 
            this.boton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton3.FlatAppearance.BorderSize = 3;
            this.boton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton3.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton3.Location = new System.Drawing.Point(265, 92);
            this.boton3.Name = "boton3";
            this.boton3.Size = new System.Drawing.Size(176, 51);
            this.boton3.TabIndex = 3;
            this.boton3.Text = "Incapacidades";
            this.boton3.UseVisualStyleBackColor = false;
            this.boton3.Click += new System.EventHandler(this.boton3_Click);
            // 
            // boton4
            // 
            this.boton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton4.FlatAppearance.BorderSize = 3;
            this.boton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton4.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton4.Location = new System.Drawing.Point(265, 163);
            this.boton4.Name = "boton4";
            this.boton4.Size = new System.Drawing.Size(176, 51);
            this.boton4.TabIndex = 4;
            this.boton4.Text = "Recomendaciones";
            this.boton4.UseVisualStyleBackColor = false;
            this.boton4.Click += new System.EventHandler(this.boton4_Click);
            // 
            // TipoOrdenMedica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 245);
            this.Controls.Add(this.boton4);
            this.Controls.Add(this.boton3);
            this.Controls.Add(this.boton2);
            this.Controls.Add(this.boton1);
            this.Name = "TipoOrdenMedica";
            this.Text = "TipoOrdenMedica";
            this.Load += new System.EventHandler(this.TipoOrdenMedica_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private FormAndControls.Controles.Boton boton1;
        private FormAndControls.Controles.Boton boton2;
        private FormAndControls.Controles.Boton boton3;
        private FormAndControls.Controles.Boton boton4;
    }
}