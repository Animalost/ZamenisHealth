namespace ZamenisHealth.Facturacion
{
    partial class ReportesCopias4
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
            this.gridZH1 = new ZamenisHealth.Clases.Controles.GridZH();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.boton1 = new FormAndControls.Controles.Boton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(139, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(741, 41);
            this.label1.TabIndex = 3;
            this.label1.Text = "Recuerde que esta opcion solo esta disponible para Facturacion electronica Hecha " +
    "en servicio de Curaciones y Consultas";
            // 
            // gridZH1
            // 
            this.gridZH1.BackColor = System.Drawing.Color.White;
            this.gridZH1.Location = new System.Drawing.Point(137, 162);
            this.gridZH1.Name = "gridZH1";
            this.gridZH1.Size = new System.Drawing.Size(751, 493);
            this.gridZH1.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ZamenisHealth.Properties.Resources.reloj_de_arena;
            this.pictureBox1.Location = new System.Drawing.Point(450, 292);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(119, 136);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(148, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Tipo de Documento";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Facturas Aseguradoras y Particulares",
            "Facturas de Recaudo de Efectivo Recepcion",
            "Facturas de Ventas Recepcion",
            "--------------------------------------------------",
            "Notas Credito"});
            this.comboBox1.Location = new System.Drawing.Point(142, 127);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(635, 28);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // boton1
            // 
            this.boton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton1.FlatAppearance.BorderSize = 3;
            this.boton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton1.Location = new System.Drawing.Point(783, 123);
            this.boton1.Name = "boton1";
            this.boton1.Size = new System.Drawing.Size(105, 32);
            this.boton1.TabIndex = 8;
            this.boton1.Text = "Cargar";
            this.boton1.UseVisualStyleBackColor = false;
            this.boton1.Click += new System.EventHandler(this.boton1_Click);
            // 
            // ReportesCopias4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 660);
            this.Controls.Add(this.boton1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.gridZH1);
            this.Controls.Add(this.label1);
            this.Name = "ReportesCopias4";
            this.Text = "ReportesCopias4";
            this.Load += new System.EventHandler(this.ReportesCopias4_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Clases.Controles.GridZH gridZH1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private FormAndControls.Controles.Boton boton1;
    }
}