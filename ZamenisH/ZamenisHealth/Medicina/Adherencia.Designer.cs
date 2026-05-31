namespace ZamenisHealth.Medicina
{
    partial class Adherencia
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Adherencia));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.boton1 = new FormAndControls.Controles.Boton();
            this.boton2 = new FormAndControls.Controles.Boton();
            this.boton3 = new FormAndControls.Controles.Boton();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 205);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox1.Size = new System.Drawing.Size(587, 324);
            this.richTextBox1.TabIndex = 19;
            this.richTextBox1.Text = "";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 132);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(587, 28);
            this.comboBox1.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(2, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(597, 72);
            this.label1.TabIndex = 15;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // boton1
            // 
            this.boton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton1.FlatAppearance.BorderSize = 3;
            this.boton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton1.Location = new System.Drawing.Point(344, 167);
            this.boton1.Name = "boton1";
            this.boton1.Size = new System.Drawing.Size(124, 32);
            this.boton1.TabIndex = 22;
            this.boton1.Text = "Agregar";
            this.boton1.UseVisualStyleBackColor = false;
            this.boton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // boton2
            // 
            this.boton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton2.FlatAppearance.BorderSize = 3;
            this.boton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton2.Location = new System.Drawing.Point(475, 166);
            this.boton2.Name = "boton2";
            this.boton2.Size = new System.Drawing.Size(124, 32);
            this.boton2.TabIndex = 23;
            this.boton2.Text = "Limpiar";
            this.boton2.UseVisualStyleBackColor = false;
            this.boton2.Click += new System.EventHandler(this.button2_Click);
            // 
            // boton3
            // 
            this.boton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.boton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.boton3.FlatAppearance.BorderSize = 3;
            this.boton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boton3.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.boton3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.boton3.Location = new System.Drawing.Point(206, 535);
            this.boton3.Name = "boton3";
            this.boton3.Size = new System.Drawing.Size(198, 32);
            this.boton3.TabIndex = 24;
            this.boton3.Text = "Agregar a la Historia";
            this.boton3.UseVisualStyleBackColor = false;
            this.boton3.Click += new System.EventHandler(this.button4_Click);
            // 
            // Adherencia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(611, 574);
            this.Controls.Add(this.boton3);
            this.Controls.Add(this.boton2);
            this.Controls.Add(this.boton1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Adherencia";
            this.Load += new System.EventHandler(this.Adherencia_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private FormAndControls.Controles.Boton boton1;
        private FormAndControls.Controles.Boton boton2;
        private FormAndControls.Controles.Boton boton3;
    }
}