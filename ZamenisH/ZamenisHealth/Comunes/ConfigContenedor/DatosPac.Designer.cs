namespace ZamenisHealth.Comunes.ConfigContenedor
{
    partial class DatosPac
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label78 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.btnZamenis1 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis2 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis3 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(12, 61);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(818, 296);
            this.richTextBox1.TabIndex = 24;
            this.richTextBox1.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(402, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(265, 26);
            this.textBox1.TabIndex = 22;
            this.textBox1.DoubleClick += new System.EventHandler(this.textBox1_DoubleClick);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 27);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(384, 28);
            this.comboBox1.TabIndex = 21;
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.BackColor = System.Drawing.Color.Transparent;
            this.label78.Location = new System.Drawing.Point(405, 11);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(70, 13);
            this.label78.TabIndex = 20;
            this.label78.Text = "Identificacion";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.BackColor = System.Drawing.Color.Transparent;
            this.label77.Location = new System.Drawing.Point(9, 11);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(94, 13);
            this.label77.TabIndex = 19;
            this.label77.Text = "Tipo Identificacion";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.Color.Blue;
            this.checkBox1.Location = new System.Drawing.Point(450, 366);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(249, 21);
            this.checkBox1.TabIndex = 28;
            this.checkBox1.Text = "Paciente de dos veces por semana";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.Visible = false;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.Color.Transparent;
            this.checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.ForeColor = System.Drawing.Color.Blue;
            this.checkBox2.Location = new System.Drawing.Point(450, 391);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(379, 21);
            this.checkBox2.TabIndex = 29;
            this.checkBox2.Text = "Paciente de varias heridas (dos espacios en la agenda)";
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.Visible = false;
            this.checkBox2.Click += new System.EventHandler(this.checkBox2_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.Color.Transparent;
            this.checkBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3.ForeColor = System.Drawing.Color.Blue;
            this.checkBox3.Location = new System.Drawing.Point(450, 416);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(233, 21);
            this.checkBox3.TabIndex = 30;
            this.checkBox3.Text = "Paciente de tratamiento especial";
            this.checkBox3.UseVisualStyleBackColor = false;
            this.checkBox3.Visible = false;
            this.checkBox3.Click += new System.EventHandler(this.checkBox3_Click);
            // 
            // btnZamenis1
            // 
            this.btnZamenis1.LabelText = "";
            this.btnZamenis1.Location = new System.Drawing.Point(687, 19);
            this.btnZamenis1.Name = "btnZamenis1";
            this.btnZamenis1.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis1.TabIndex = 31;
            // 
            // btnZamenis2
            // 
            this.btnZamenis2.LabelText = "";
            this.btnZamenis2.Location = new System.Drawing.Point(12, 402);
            this.btnZamenis2.Name = "btnZamenis2";
            this.btnZamenis2.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis2.TabIndex = 32;
            // 
            // btnZamenis3
            // 
            this.btnZamenis3.LabelText = "";
            this.btnZamenis3.Location = new System.Drawing.Point(169, 402);
            this.btnZamenis3.Name = "btnZamenis3";
            this.btnZamenis3.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis3.TabIndex = 33;
            // 
            // DatosPac
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(840, 449);
            this.ControlBox = false;
            this.Controls.Add(this.btnZamenis3);
            this.Controls.Add(this.btnZamenis2);
            this.Controls.Add(this.btnZamenis1);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label78);
            this.Controls.Add(this.label77);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "DatosPac";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.DatosPac_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label78;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private ControlUser.Controles.btnZamenis btnZamenis1;
        private ControlUser.Controles.btnZamenis btnZamenis2;
        private ControlUser.Controles.btnZamenis btnZamenis3;
    }
}