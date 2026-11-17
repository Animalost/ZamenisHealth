namespace ZamenisHealth.Recepcion.AgendaDiaria
{
    partial class HistorialAdmisiones
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.boton1 = new FormAndControls.Controles.Boton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gridZH1 = new ZamenisHealth.Clases.Controles.GridZH();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filtro";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(8, 83);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(973, 26);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // boton1
            // 
            this.boton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton1.FlatAppearance.BorderSize = 3;
            this.boton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton1.Location = new System.Drawing.Point(987, 78);
            this.boton1.Name = "boton1";
            this.boton1.Size = new System.Drawing.Size(124, 32);
            this.boton1.TabIndex = 4;
            this.boton1.Text = "Recargar";
            this.boton1.UseVisualStyleBackColor = false;
            this.boton1.Click += new System.EventHandler(this.boton1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ZamenisHealth.Properties.Resources.reloj_de_arena;
            this.pictureBox1.Location = new System.Drawing.Point(488, 286);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(123, 125);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // gridZH1
            // 
            this.gridZH1.BackColor = System.Drawing.Color.White;
            this.gridZH1.Location = new System.Drawing.Point(5, 114);
            this.gridZH1.Name = "gridZH1";
            this.gridZH1.Size = new System.Drawing.Size(1106, 482);
            this.gridZH1.TabIndex = 1;
            // 
            // HistorialAdmisiones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1116, 601);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.boton1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridZH1);
            this.Name = "HistorialAdmisiones";
            this.Text = "HistorialAdmisiones";
            this.Load += new System.EventHandler(this.HistorialAdmisiones_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Clases.Controles.GridZH gridZH1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private FormAndControls.Controles.Boton boton1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}