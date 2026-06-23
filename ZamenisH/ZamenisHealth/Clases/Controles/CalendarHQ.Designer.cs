namespace ZamenisHealth.Clases.Controles
{
    partial class CalendarHQ
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
            this.panelCal = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CalControlDGV = new System.Windows.Forms.DataGridView();
            this.btnHoyReturn = new System.Windows.Forms.Button();
            this.cmbMes = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFecha = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbAño = new System.Windows.Forms.ComboBox();
            this.btnPreviusMonth = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDia = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dTPCalendar = new System.Windows.Forms.DateTimePicker();
            this.panelCal.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CalControlDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panelCal
            // 
            this.panelCal.BackColor = System.Drawing.Color.Navy;
            this.panelCal.Controls.Add(this.panel1);
            this.panelCal.Controls.Add(this.textBox1);
            this.panelCal.Controls.Add(this.dTPCalendar);
            this.panelCal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCal.Location = new System.Drawing.Point(0, 0);
            this.panelCal.Name = "panelCal";
            this.panelCal.Size = new System.Drawing.Size(396, 465);
            this.panelCal.TabIndex = 165;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.CalControlDGV);
            this.panel1.Controls.Add(this.btnHoyReturn);
            this.panel1.Controls.Add(this.cmbMes);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.txtFecha);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.cmbAño);
            this.panel1.Controls.Add(this.btnPreviusMonth);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtDia);
            this.panel1.Location = new System.Drawing.Point(12, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(372, 438);
            this.panel1.TabIndex = 157;
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
            this.CalControlDGV.Location = new System.Drawing.Point(9, 85);
            this.CalControlDGV.MultiSelect = false;
            this.CalControlDGV.Name = "CalControlDGV";
            this.CalControlDGV.ReadOnly = true;
            this.CalControlDGV.RowHeadersVisible = false;
            this.CalControlDGV.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.CalControlDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.CalControlDGV.Size = new System.Drawing.Size(353, 268);
            this.CalControlDGV.TabIndex = 148;
            // 
            // btnHoyReturn
            // 
            this.btnHoyReturn.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnHoyReturn.BackgroundImage = global::ZamenisHealth.Properties.Resources.CalToday;
            this.btnHoyReturn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHoyReturn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHoyReturn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoyReturn.ForeColor = System.Drawing.Color.White;
            this.btnHoyReturn.Location = new System.Drawing.Point(153, 363);
            this.btnHoyReturn.Name = "btnHoyReturn";
            this.btnHoyReturn.Size = new System.Drawing.Size(69, 68);
            this.btnHoyReturn.TabIndex = 155;
            this.btnHoyReturn.UseVisualStyleBackColor = false;
            this.btnHoyReturn.Click += new System.EventHandler(this.btnHoyReturn_Click);
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
            this.cmbMes.Location = new System.Drawing.Point(147, 46);
            this.cmbMes.Name = "cmbMes";
            this.cmbMes.Size = new System.Drawing.Size(167, 28);
            this.cmbMes.TabIndex = 146;
            this.cmbMes.SelectedIndexChanged += new System.EventHandler(this.cmbMes_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.SteelBlue;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(3, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(367, 23);
            this.button3.TabIndex = 157;
            this.button3.Text = "CALENDARIO";
            this.button3.UseVisualStyleBackColor = false;
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
            this.button1.Location = new System.Drawing.Point(325, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(37, 38);
            this.button1.TabIndex = 154;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtFecha
            // 
            this.txtFecha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFecha.Cursor = System.Windows.Forms.Cursors.No;
            this.txtFecha.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFecha.ForeColor = System.Drawing.Color.Blue;
            this.txtFecha.Location = new System.Drawing.Point(7, 382);
            this.txtFecha.Name = "txtFecha";
            this.txtFecha.ReadOnly = true;
            this.txtFecha.Size = new System.Drawing.Size(135, 26);
            this.txtFecha.TabIndex = 143;
            this.txtFecha.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(54, 366);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 149;
            this.label1.Text = "Fecha";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(79, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 151;
            this.label3.Text = "Año";
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
            this.cmbAño.Location = new System.Drawing.Point(51, 46);
            this.cmbAño.Name = "cmbAño";
            this.cmbAño.Size = new System.Drawing.Size(87, 28);
            this.cmbAño.TabIndex = 145;
            this.cmbAño.SelectedIndexChanged += new System.EventHandler(this.cmbAño_SelectedIndexChanged);
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
            this.btnPreviusMonth.Location = new System.Drawing.Point(7, 39);
            this.btnPreviusMonth.Name = "btnPreviusMonth";
            this.btnPreviusMonth.Size = new System.Drawing.Size(37, 38);
            this.btnPreviusMonth.TabIndex = 153;
            this.btnPreviusMonth.UseVisualStyleBackColor = false;
            this.btnPreviusMonth.Click += new System.EventHandler(this.btnPreviusMonth_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(216, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 152;
            this.label4.Text = "Mes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(284, 366);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 150;
            this.label2.Text = "Dia";
            // 
            // txtDia
            // 
            this.txtDia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDia.Cursor = System.Windows.Forms.Cursors.No;
            this.txtDia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDia.ForeColor = System.Drawing.Color.Blue;
            this.txtDia.Location = new System.Drawing.Point(229, 382);
            this.txtDia.Name = "txtDia";
            this.txtDia.ReadOnly = true;
            this.txtDia.Size = new System.Drawing.Size(135, 26);
            this.txtDia.TabIndex = 144;
            this.txtDia.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(63, 526);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(277, 20);
            this.textBox1.TabIndex = 156;
            // 
            // dTPCalendar
            // 
            this.dTPCalendar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dTPCalendar.Location = new System.Drawing.Point(63, 494);
            this.dTPCalendar.Name = "dTPCalendar";
            this.dTPCalendar.Size = new System.Drawing.Size(277, 26);
            this.dTPCalendar.TabIndex = 147;
            // 
            // CalendarHQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelCal);
            this.Name = "CalendarHQ";
            this.Size = new System.Drawing.Size(396, 465);
            this.Load += new System.EventHandler(this.CalendarHQ_Load);
            this.panelCal.ResumeLayout(false);
            this.panelCal.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CalControlDGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panelCal;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.DataGridView CalControlDGV;
        public System.Windows.Forms.Button btnHoyReturn;
        public System.Windows.Forms.ComboBox cmbMes;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox txtFecha;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox cmbAño;
        public System.Windows.Forms.Button btnPreviusMonth;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtDia;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.DateTimePicker dTPCalendar;
    }
}
