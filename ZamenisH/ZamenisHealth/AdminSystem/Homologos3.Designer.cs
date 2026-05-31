namespace ZamenisHealth.AdminSystem
{
    partial class Homologos3
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnZamenis1 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis2 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis3 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(13, 125);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(836, 377);
            this.dataGridView1.TabIndex = 13;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ZamenisHealth.Properties.Resources2.reloj_de_arena;
            this.pictureBox2.Location = new System.Drawing.Point(39, 72);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(42, 36);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "",
            "Factura de Servicios",
            "Factura de Ventas de Productos",
            "Recibos de Caja",
            "Inventario",
            "Pagos"});
            this.comboBox1.Location = new System.Drawing.Point(119, 48);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(730, 28);
            this.comboBox1.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Tipo de Documento";
            // 
            // btnZamenis1
            // 
            this.btnZamenis1.LabelText = "";
            this.btnZamenis1.Location = new System.Drawing.Point(119, 83);
            this.btnZamenis1.Name = "btnZamenis1";
            this.btnZamenis1.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis1.TabIndex = 23;
            // 
            // btnZamenis2
            // 
            this.btnZamenis2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZamenis2.LabelText = "";
            this.btnZamenis2.Location = new System.Drawing.Point(553, 83);
            this.btnZamenis2.Name = "btnZamenis2";
            this.btnZamenis2.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis2.TabIndex = 24;
            // 
            // btnZamenis3
            // 
            this.btnZamenis3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZamenis3.Enabled = false;
            this.btnZamenis3.LabelText = "";
            this.btnZamenis3.Location = new System.Drawing.Point(706, 83);
            this.btnZamenis3.Name = "btnZamenis3";
            this.btnZamenis3.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis3.TabIndex = 25;
            // 
            // Homologos3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(861, 514);
            this.Controls.Add(this.btnZamenis3);
            this.Controls.Add(this.btnZamenis2);
            this.Controls.Add(this.btnZamenis1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Homologos3";
            this.Load += new System.EventHandler(this.Homologos3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private ControlUser.Controles.btnZamenis btnZamenis1;
        private ControlUser.Controles.btnZamenis btnZamenis2;
        private ControlUser.Controles.btnZamenis btnZamenis3;
    }
}