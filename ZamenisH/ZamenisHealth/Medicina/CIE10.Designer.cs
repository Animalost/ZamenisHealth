namespace ZamenisHealth.Medicina
{
    partial class CIE10
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
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gridZH1 = new ZamenisHealth.Clases.Controles.GridZH();
            this.gridZH2 = new ZamenisHealth.Clases.Controles.GridZH();
            this.boton1 = new FormAndControls.Controles.Boton();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 17);
            this.label4.TabIndex = 34;
            this.label4.Text = "Criterio de Busqueda";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(9, 125);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(598, 26);
            this.textBox1.TabIndex = 29;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "",
            "Codigo CIE10",
            "Descripcion"});
            this.comboBox1.Location = new System.Drawing.Point(9, 77);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(468, 28);
            this.comboBox1.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 17);
            this.label2.TabIndex = 27;
            this.label2.Text = "Metodo de Busqueda";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 413);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(195, 17);
            this.label5.TabIndex = 38;
            this.label5.Text = "Sus diagnosticos mas usados";
            // 
            // gridZH1
            // 
            this.gridZH1.BackColor = System.Drawing.Color.White;
            this.gridZH1.Location = new System.Drawing.Point(9, 157);
            this.gridZH1.Name = "gridZH1";
            this.gridZH1.Size = new System.Drawing.Size(598, 254);
            this.gridZH1.TabIndex = 39;
            // 
            // gridZH2
            // 
            this.gridZH2.BackColor = System.Drawing.Color.White;
            this.gridZH2.Location = new System.Drawing.Point(9, 435);
            this.gridZH2.Name = "gridZH2";
            this.gridZH2.Size = new System.Drawing.Size(598, 255);
            this.gridZH2.TabIndex = 40;
            // 
            // boton1
            // 
            this.boton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton1.FlatAppearance.BorderSize = 3;
            this.boton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton1.Location = new System.Drawing.Point(483, 76);
            this.boton1.Name = "boton1";
            this.boton1.Size = new System.Drawing.Size(124, 32);
            this.boton1.TabIndex = 41;
            this.boton1.Text = "Buscar CIE10";
            this.boton1.UseVisualStyleBackColor = false;
            this.boton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CIE10
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(618, 695);
            this.Controls.Add(this.boton1);
            this.Controls.Add(this.gridZH2);
            this.Controls.Add(this.gridZH1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CIE10";
            this.Load += new System.EventHandler(this.CIE10_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private Clases.Controles.GridZH gridZH1;
        private Clases.Controles.GridZH gridZH2;
        private FormAndControls.Controles.Boton boton1;
    }
}