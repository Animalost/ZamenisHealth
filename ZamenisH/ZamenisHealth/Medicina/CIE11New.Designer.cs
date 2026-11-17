namespace ZamenisHealth.Medicina
{
    partial class CIE11New
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
            this.boton2 = new FormAndControls.Controles.Boton();
            this.gridZH3 = new ZamenisHealth.Clases.Controles.GridZH();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // boton2
            // 
            this.boton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton2.FlatAppearance.BorderSize = 3;
            this.boton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton2.Location = new System.Drawing.Point(486, 119);
            this.boton2.Name = "boton2";
            this.boton2.Size = new System.Drawing.Size(124, 32);
            this.boton2.TabIndex = 51;
            this.boton2.Text = "Buscar CIE11";
            this.boton2.UseVisualStyleBackColor = false;
            this.boton2.Click += new System.EventHandler(this.boton2_Click);
            // 
            // gridZH3
            // 
            this.gridZH3.BackColor = System.Drawing.Color.White;
            this.gridZH3.Location = new System.Drawing.Point(12, 154);
            this.gridZH3.Name = "gridZH3";
            this.gridZH3.Size = new System.Drawing.Size(598, 437);
            this.gridZH3.TabIndex = 50;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 17);
            this.label3.TabIndex = 49;
            this.label3.Text = "Descripcion del diagnostico";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(12, 122);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(469, 26);
            this.textBox2.TabIndex = 48;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(601, 48);
            this.label1.TabIndex = 47;
            this.label1.Text = "La normatividad Colombiana exige asociar el diagnostico CIE10 seleccionado con el" +
    " nuevo diagnostico CIE11, seleccione el diagnostico principal CIE11";
            // 
            // CIE11New
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 603);
            this.Controls.Add(this.boton2);
            this.Controls.Add(this.gridZH3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Name = "CIE11New";
            this.Text = "CIE11New";
            this.Load += new System.EventHandler(this.CIE11New_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FormAndControls.Controles.Boton boton2;
        private Clases.Controles.GridZH gridZH3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
    }
}