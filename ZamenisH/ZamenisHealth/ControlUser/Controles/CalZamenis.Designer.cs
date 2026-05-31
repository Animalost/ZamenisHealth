namespace ZamenisHealth.ControlUser.Controles
{
    partial class CalZamenis
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalZamenis));
            this.btnHoyReturn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnPreviusMonth = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CalControlDGV = new System.Windows.Forms.DataGridView();
            this.dTPCalendar = new System.Windows.Forms.DateTimePicker();
            this.cmbMes = new System.Windows.Forms.ComboBox();
            this.cmbAño = new System.Windows.Forms.ComboBox();
            this.txtDia = new System.Windows.Forms.TextBox();
            this.txtFecha = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CalControlDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnHoyReturn
            // 
            this.btnHoyReturn.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnHoyReturn.BackgroundImage = global::ZamenisHealth.Properties.Resources.CalToday;
            this.btnHoyReturn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHoyReturn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHoyReturn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoyReturn.ForeColor = System.Drawing.Color.White;
            this.btnHoyReturn.Location = new System.Drawing.Point(159, 386);
            this.btnHoyReturn.Name = "btnHoyReturn";
            this.btnHoyReturn.Size = new System.Drawing.Size(69, 68);
            this.btnHoyReturn.TabIndex = 133;
            this.btnHoyReturn.UseVisualStyleBackColor = false;
            this.btnHoyReturn.Click += new System.EventHandler(this.btnHoyReturn_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.RoyalBlue;
            this.button1.BackgroundImage = global::ZamenisHealth.Properties.Resources.CalMas;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(327, 57);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(37, 38);
            this.button1.TabIndex = 132;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnPreviusMonth
            // 
            this.btnPreviusMonth.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnPreviusMonth.BackgroundImage = global::ZamenisHealth.Properties.Resources.CalMinus;
            this.btnPreviusMonth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPreviusMonth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreviusMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreviusMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreviusMonth.ForeColor = System.Drawing.Color.White;
            this.btnPreviusMonth.Location = new System.Drawing.Point(13, 57);
            this.btnPreviusMonth.Name = "btnPreviusMonth";
            this.btnPreviusMonth.Size = new System.Drawing.Size(37, 38);
            this.btnPreviusMonth.TabIndex = 131;
            this.btnPreviusMonth.UseVisualStyleBackColor = false;
            this.btnPreviusMonth.Click += new System.EventHandler(this.btnPreviusMonth_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(227, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 130;
            this.label4.Text = "Mes";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(88, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 129;
            this.label3.Text = "Año";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(290, 384);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "Dia";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(60, 384);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 127;
            this.label1.Text = "Fecha";
            // 
            // CalControlDGV
            // 
            this.CalControlDGV.AllowUserToAddRows = false;
            this.CalControlDGV.AllowUserToDeleteRows = false;
            this.CalControlDGV.AllowUserToResizeColumns = false;
            this.CalControlDGV.AllowUserToResizeRows = false;
            this.CalControlDGV.BackgroundColor = System.Drawing.Color.White;
            this.CalControlDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CalControlDGV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CalControlDGV.GridColor = System.Drawing.Color.Blue;
            this.CalControlDGV.Location = new System.Drawing.Point(15, 103);
            this.CalControlDGV.MultiSelect = false;
            this.CalControlDGV.Name = "CalControlDGV";
            this.CalControlDGV.ReadOnly = true;
            this.CalControlDGV.RowHeadersVisible = false;
            this.CalControlDGV.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.CalControlDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.CalControlDGV.Size = new System.Drawing.Size(353, 268);
            this.CalControlDGV.TabIndex = 126;
            this.CalControlDGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CalControlDGV_CellClick);
            // 
            // dTPCalendar
            // 
            this.dTPCalendar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dTPCalendar.Location = new System.Drawing.Point(13, 578);
            this.dTPCalendar.Name = "dTPCalendar";
            this.dTPCalendar.Size = new System.Drawing.Size(277, 26);
            this.dTPCalendar.TabIndex = 125;
            // 
            // cmbMes
            // 
            this.cmbMes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbMes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMes.FormattingEnabled = true;
            this.cmbMes.Items.AddRange(new object[] {
            "ENERO",
            "FEBRERO",
            "MARZO",
            "ABRIL",
            "MAYO",
            "JUNIO",
            "JULIO",
            "AGOSTO",
            "SEPTIEMBRE",
            "OCTUBRE",
            "NOVIEMBRE",
            "DICIEMBRE"});
            this.cmbMes.Location = new System.Drawing.Point(153, 64);
            this.cmbMes.Name = "cmbMes";
            this.cmbMes.Size = new System.Drawing.Size(167, 28);
            this.cmbMes.TabIndex = 124;
            this.cmbMes.SelectedIndexChanged += new System.EventHandler(this.cmbMes_SelectedIndexChanged);
            this.cmbMes.Leave += new System.EventHandler(this.cmbMes_Leave);
            // 
            // cmbAño
            // 
            this.cmbAño.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbAño.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAño.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAño.FormattingEnabled = true;
            this.cmbAño.Items.AddRange(new object[] {
            "2024",
            "2019",
            "2020",
            "2021",
            "2022",
            "2023",
            "2025",
            "2026",
            "2027",
            "2028",
            "2029",
            "2030"});
            this.cmbAño.Location = new System.Drawing.Point(57, 64);
            this.cmbAño.Name = "cmbAño";
            this.cmbAño.Size = new System.Drawing.Size(87, 28);
            this.cmbAño.TabIndex = 123;
            this.cmbAño.SelectedIndexChanged += new System.EventHandler(this.cmbAño_SelectedIndexChanged);
            this.cmbAño.Leave += new System.EventHandler(this.cmbAño_Leave);
            // 
            // txtDia
            // 
            this.txtDia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDia.Cursor = System.Windows.Forms.Cursors.No;
            this.txtDia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDia.ForeColor = System.Drawing.Color.Blue;
            this.txtDia.Location = new System.Drawing.Point(235, 400);
            this.txtDia.Name = "txtDia";
            this.txtDia.ReadOnly = true;
            this.txtDia.Size = new System.Drawing.Size(135, 26);
            this.txtDia.TabIndex = 122;
            this.txtDia.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtFecha
            // 
            this.txtFecha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFecha.Cursor = System.Windows.Forms.Cursors.No;
            this.txtFecha.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFecha.ForeColor = System.Drawing.Color.Blue;
            this.txtFecha.Location = new System.Drawing.Point(13, 400);
            this.txtFecha.Name = "txtFecha";
            this.txtFecha.ReadOnly = true;
            this.txtFecha.Size = new System.Drawing.Size(135, 26);
            this.txtFecha.TabIndex = 121;
            this.txtFecha.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(296, 571);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 134;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.SteelBlue;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(9, 23);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(367, 23);
            this.button3.TabIndex = 136;
            this.button3.Text = "CALENDARIO";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.SteelBlue;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(349, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(23, 23);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 137;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Navy;
            this.label5.Location = new System.Drawing.Point(0, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 479);
            this.label5.TabIndex = 138;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Navy;
            this.label6.Location = new System.Drawing.Point(373, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 477);
            this.label6.TabIndex = 139;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Navy;
            this.label7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label7.Location = new System.Drawing.Point(-1, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(377, 24);
            this.label7.TabIndex = 140;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Navy;
            this.label8.Location = new System.Drawing.Point(8, 457);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(368, 10);
            this.label8.TabIndex = 141;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Navy;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(9, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(375, 20);
            this.label9.TabIndex = 142;
            this.label9.Text = "DesAnclado";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // CalZamenis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnHoyReturn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnPreviusMonth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CalControlDGV);
            this.Controls.Add(this.dTPCalendar);
            this.Controls.Add(this.cmbMes);
            this.Controls.Add(this.cmbAño);
            this.Controls.Add(this.txtDia);
            this.Controls.Add(this.txtFecha);
            this.DoubleBuffered = true;
            this.Name = "CalZamenis";
            this.Size = new System.Drawing.Size(383, 465);
            this.Load += new System.EventHandler(this.CalZamenis_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CalControlDGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnHoyReturn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnPreviusMonth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMes;
        private System.Windows.Forms.ComboBox cmbAño;
        public System.Windows.Forms.DateTimePicker dTPCalendar;
        public System.Windows.Forms.TextBox txtDia;
        public System.Windows.Forms.TextBox txtFecha;
        public System.Windows.Forms.DataGridView CalControlDGV;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}
