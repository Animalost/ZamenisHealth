namespace ZamenisHealth.AdminSystem
{
    partial class Homologos
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
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnZamenis1 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis3 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.SuspendLayout();
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(11, 65);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(524, 28);
            this.comboBox2.TabIndex = 44;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(8, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Seleccione prestador a consultar";
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(11, 193);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(673, 278);
            this.listView1.TabIndex = 42;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Facturacion General de Aseguradoras",
            "Facturacion de Ventas"});
            this.comboBox1.Location = new System.Drawing.Point(11, 117);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(673, 28);
            this.comboBox1.TabIndex = 41;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(8, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(590, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "Escoja el tipo de factura que desea crearle el numero homologo, esta informacion " +
    "es de vital importancia para fines de RIPS";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(182, 158);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(353, 26);
            this.textBox1.TabIndex = 37;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Digite numero de factura zamenis";
            // 
            // btnZamenis1
            // 
            this.btnZamenis1.LabelText = "";
            this.btnZamenis1.Location = new System.Drawing.Point(541, 60);
            this.btnZamenis1.Name = "btnZamenis1";
            this.btnZamenis1.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis1.TabIndex = 46;
            // 
            // btnZamenis3
            // 
            this.btnZamenis3.LabelText = "";
            this.btnZamenis3.Location = new System.Drawing.Point(541, 151);
            this.btnZamenis3.Name = "btnZamenis3";
            this.btnZamenis3.Size = new System.Drawing.Size(143, 36);
            this.btnZamenis3.TabIndex = 48;
            // 
            // Homologos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(693, 479);
            this.Controls.Add(this.btnZamenis3);
            this.Controls.Add(this.btnZamenis1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Name = "Homologos";
            this.Load += new System.EventHandler(this.Homologos_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private ControlUser.Controles.btnZamenis btnZamenis1;
        private ControlUser.Controles.btnZamenis btnZamenis3;
    }
}