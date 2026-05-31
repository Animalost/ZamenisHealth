namespace ZamenisHealth.Mensajeria
{
    partial class Sender
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
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnZamenis1 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            this.btnZamenis2 = new ZamenisHealth.ControlUser.Controles.btnZamenis();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(3, 178);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(170, 20);
            this.label6.TabIndex = 56;
            this.label6.Text = "Seleccione su desision";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Location = new System.Drawing.Point(7, 131);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(524, 26);
            this.dateTimePicker1.TabIndex = 55;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(4, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(294, 13);
            this.label5.TabIndex = 54;
            this.label5.Text = "Seleccione la fecha del dia que se enviaran los recordatorios";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(4, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 13);
            this.label1.TabIndex = 50;
            this.label1.Text = "Seleccione la clase de cita a la que desea enviar el recordatorio";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "",
            "Citas de Curaciones",
            "Citas de Medicina General",
            "Citas de Fisiatria",
            "Citas de Piscologia",
            "Citas de Terapia Fisica",
            "Citas de Terapia Ocupacional",
            "Citas de Radiologia"});
            this.comboBox2.Location = new System.Drawing.Point(5, 70);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(526, 28);
            this.comboBox2.TabIndex = 49;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ZamenisHealth.Properties.Resources2.reloj_de_arena;
            this.pictureBox2.Location = new System.Drawing.Point(419, 184);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(112, 106);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 58;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // btnZamenis1
            // 
            this.btnZamenis1.LabelText = "";
            this.btnZamenis1.Location = new System.Drawing.Point(5, 212);
            this.btnZamenis1.Name = "btnZamenis1";
            this.btnZamenis1.Size = new System.Drawing.Size(199, 36);
            this.btnZamenis1.TabIndex = 59;
            // 
            // btnZamenis2
            // 
            this.btnZamenis2.LabelText = "";
            this.btnZamenis2.Location = new System.Drawing.Point(5, 254);
            this.btnZamenis2.Name = "btnZamenis2";
            this.btnZamenis2.Size = new System.Drawing.Size(199, 36);
            this.btnZamenis2.TabIndex = 60;
            // 
            // Sender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(537, 300);
            this.Controls.Add(this.btnZamenis2);
            this.Controls.Add(this.btnZamenis1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Sender";
            this.Load += new System.EventHandler(this.Sender_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private ControlUser.Controles.btnZamenis btnZamenis1;
        private ControlUser.Controles.btnZamenis btnZamenis2;
    }
}