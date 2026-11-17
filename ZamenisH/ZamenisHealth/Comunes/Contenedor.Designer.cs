using Persistence;

namespace ZamenisHealth.Comunes
{
    partial class Contenedor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Contenedor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.CerrarPanel = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label73 = new System.Windows.Forms.Label();
            this.label72 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label78 = new System.Windows.Forms.Label();
            this.cbVoice = new System.Windows.Forms.ComboBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.linkLabel7 = new System.Windows.Forms.LinkLabel();
            this.linkLabel8 = new System.Windows.Forms.LinkLabel();
            this.linkLabel11 = new System.Windows.Forms.LinkLabel();
            this.linkLabel12 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel15 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel16 = new System.Windows.Forms.LinkLabel();
            this.linkLabel17 = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new FormAndControls.Controles.Boton();
            this.toolStripButton1 = new FormAndControls.Controles.Boton();
            this.toolStripButton12 = new FormAndControls.Controles.Boton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Location = new System.Drawing.Point(134, 102);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(903, 516);
            this.panel1.TabIndex = 67;
            this.panel1.Visible = false;
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Interval = 15000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // CerrarPanel
            // 
            this.CerrarPanel.Interval = 15000;
            this.CerrarPanel.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label73
            // 
            this.label73.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label73.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label73.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label73.Location = new System.Drawing.Point(646, 38);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(286, 23);
            this.label73.TabIndex = 65;
            this.label73.Text = " / / ";
            this.label73.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label73.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStrip2_MouseDown);
            // 
            // label72
            // 
            this.label72.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label72.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label72.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label72.Location = new System.Drawing.Point(763, 11);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(169, 23);
            this.label72.TabIndex = 64;
            this.label72.Text = "--:--:--";
            this.label72.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label72.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStrip2_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(151, 268);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(261, 17);
            this.label2.TabIndex = 70;
            this.label2.Text = "CLICK AQUI para ver conocer ultimos anuncios";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox4.BackColor = System.Drawing.Color.White;
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = global::ZamenisHealth.Properties.Resources2.paciente;
            this.pictureBox4.Location = new System.Drawing.Point(150, 14);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(44, 44);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 74;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.DoubleClick += new System.EventHandler(this.pictureBox4_DoubleClick);
            // 
            // label78
            // 
            this.label78.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label78.AutoSize = true;
            this.label78.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label78.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label78.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label78.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label78.Location = new System.Drawing.Point(198, 26);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(24, 21);
            this.label78.TabIndex = 270;
            this.label78.Text = "➤";
            this.label78.Click += new System.EventHandler(this.label78_Click);
            // 
            // cbVoice
            // 
            this.cbVoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVoice.FormattingEnabled = true;
            this.cbVoice.Location = new System.Drawing.Point(1053, 148);
            this.cbVoice.Name = "cbVoice";
            this.cbVoice.Size = new System.Drawing.Size(121, 21);
            this.cbVoice.TabIndex = 271;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel2.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel2.Location = new System.Drawing.Point(148, 121);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(85, 13);
            this.linkLabel2.TabIndex = 275;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "https://slsoft.net";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.ForeColor = System.Drawing.Color.Blue;
            this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel1.Location = new System.Drawing.Point(148, 120);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(102, 13);
            this.linkLabel1.TabIndex = 276;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "App Celular y PC";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel3.LinkColor = System.Drawing.Color.Black;
            this.linkLabel3.Location = new System.Drawing.Point(261, 120);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(145, 13);
            this.linkLabel3.TabIndex = 277;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "<--- Ingresar App Celular";
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel4.LinkColor = System.Drawing.Color.Black;
            this.linkLabel4.Location = new System.Drawing.Point(261, 121);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(111, 13);
            this.linkLabel4.TabIndex = 278;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "<--- Visitar Web Oficial";
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel5.ForeColor = System.Drawing.Color.Blue;
            this.linkLabel5.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel5.Location = new System.Drawing.Point(148, 148);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(109, 13);
            this.linkLabel5.TabIndex = 279;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "Activar Reloj Analogo";
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel5_LinkClicked);
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel6.LinkColor = System.Drawing.Color.Black;
            this.linkLabel6.Location = new System.Drawing.Point(261, 148);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(134, 13);
            this.linkLabel6.TabIndex = 280;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "<--- Habilita el reloj analogo";
            // 
            // linkLabel7
            // 
            this.linkLabel7.AutoSize = true;
            this.linkLabel7.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel7.LinkColor = System.Drawing.Color.Black;
            this.linkLabel7.Location = new System.Drawing.Point(261, 177);
            this.linkLabel7.Name = "linkLabel7";
            this.linkLabel7.Size = new System.Drawing.Size(161, 13);
            this.linkLabel7.TabIndex = 282;
            this.linkLabel7.TabStop = true;
            this.linkLabel7.Text = "<--- Ver Cambios de la aplicacion";
            // 
            // linkLabel8
            // 
            this.linkLabel8.AutoSize = true;
            this.linkLabel8.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel8.ForeColor = System.Drawing.Color.Blue;
            this.linkLabel8.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel8.Location = new System.Drawing.Point(148, 177);
            this.linkLabel8.Name = "linkLabel8";
            this.linkLabel8.Size = new System.Drawing.Size(105, 13);
            this.linkLabel8.TabIndex = 281;
            this.linkLabel8.TabStop = true;
            this.linkLabel8.Text = "Novedades Zamenis";
            this.linkLabel8.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel8_LinkClicked);
            // 
            // linkLabel11
            // 
            this.linkLabel11.AutoSize = true;
            this.linkLabel11.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel11.LinkColor = System.Drawing.Color.Black;
            this.linkLabel11.Location = new System.Drawing.Point(261, 206);
            this.linkLabel11.Name = "linkLabel11";
            this.linkLabel11.Size = new System.Drawing.Size(128, 13);
            this.linkLabel11.TabIndex = 285;
            this.linkLabel11.TabStop = true;
            this.linkLabel11.Text = "<--- Manuales Disponibles";
            this.linkLabel11.Visible = false;
            // 
            // linkLabel12
            // 
            this.linkLabel12.AutoSize = true;
            this.linkLabel12.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel12.ForeColor = System.Drawing.Color.Blue;
            this.linkLabel12.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel12.Location = new System.Drawing.Point(148, 206);
            this.linkLabel12.Name = "linkLabel12";
            this.linkLabel12.Size = new System.Drawing.Size(72, 13);
            this.linkLabel12.TabIndex = 286;
            this.linkLabel12.TabStop = true;
            this.linkLabel12.Text = "Ver Manuales";
            this.linkLabel12.Visible = false;
            this.linkLabel12.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel12_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DarkMagenta;
            this.label4.Location = new System.Drawing.Point(895, 629);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 9);
            this.label4.TabIndex = 72;
            this.label4.Text = "En Produccion Local";
            // 
            // linkLabel15
            // 
            this.linkLabel15.AutoSize = true;
            this.linkLabel15.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel15.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel15.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel15.LinkColor = System.Drawing.Color.Red;
            this.linkLabel15.Location = new System.Drawing.Point(148, 298);
            this.linkLabel15.Name = "linkLabel15";
            this.linkLabel15.Size = new System.Drawing.Size(214, 17);
            this.linkLabel15.TabIndex = 293;
            this.linkLabel15.TabStop = true;
            this.linkLabel15.Text = "Habilitar Mensajero Nuevamente";
            this.linkLabel15.Visible = false;
            this.linkLabel15.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel15_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(824, 579);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(201, 20);
            this.label3.TabIndex = 294;
            this.label3.Text = "APLICACION EN MORA";
            // 
            // linkLabel16
            // 
            this.linkLabel16.AutoSize = true;
            this.linkLabel16.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel16.LinkColor = System.Drawing.Color.Black;
            this.linkLabel16.Location = new System.Drawing.Point(261, 235);
            this.linkLabel16.Name = "linkLabel16";
            this.linkLabel16.Size = new System.Drawing.Size(499, 13);
            this.linkLabel16.TabIndex = 295;
            this.linkLabel16.TabStop = true;
            this.linkLabel16.Text = "<--- Puede abrir el mensajero presionando Shift F12 en cualquier parte de la apli" +
    "cacion";
            // 
            // linkLabel17
            // 
            this.linkLabel17.AutoSize = true;
            this.linkLabel17.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel17.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel17.ForeColor = System.Drawing.Color.Blue;
            this.linkLabel17.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.linkLabel17.Location = new System.Drawing.Point(148, 235);
            this.linkLabel17.Name = "linkLabel17";
            this.linkLabel17.Size = new System.Drawing.Size(65, 13);
            this.linkLabel17.TabIndex = 296;
            this.linkLabel17.TabStop = true;
            this.linkLabel17.Text = "Mensajero";
            this.linkLabel17.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel17_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label5.Font = new System.Drawing.Font("Arial Narrow", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.MediumBlue;
            this.label5.Location = new System.Drawing.Point(139, 621);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 17);
            this.label5.TabIndex = 297;
            this.label5.Text = "Sin Novedades";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.label7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label7.Location = new System.Drawing.Point(4, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 23);
            this.label7.TabIndex = 301;
            this.label7.Text = "Zamenis Health";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::ZamenisHealth.Properties.Resources.cerca;
            this.pictureBox2.Location = new System.Drawing.Point(1051, 102);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(24, 24);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 305;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.toolStripButton1);
            this.panel2.Controls.Add(this.toolStripButton12);
            this.panel2.Location = new System.Drawing.Point(133, 66);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(904, 37);
            this.panel2.TabIndex = 306;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 3;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.button1.Location = new System.Drawing.Point(467, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(424, 32);
            this.button1.TabIndex = 309;
            this.button1.Text = "¿Desea contribuir al desarrollo de esta aplicacion, Click Aqui?";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.label3_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.toolStripButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toolStripButton1.FlatAppearance.BorderSize = 3;
            this.toolStripButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripButton1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.toolStripButton1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.toolStripButton1.Location = new System.Drawing.Point(9, 0);
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(124, 32);
            this.toolStripButton1.TabIndex = 307;
            this.toolStripButton1.Text = "Herramientas";
            this.toolStripButton1.UseVisualStyleBackColor = false;
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButtonMenu1_Click);
            // 
            // toolStripButton12
            // 
            this.toolStripButton12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.toolStripButton12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toolStripButton12.FlatAppearance.BorderSize = 3;
            this.toolStripButton12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripButton12.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.toolStripButton12.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.toolStripButton12.Location = new System.Drawing.Point(139, 0);
            this.toolStripButton12.Name = "toolStripButton12";
            this.toolStripButton12.Size = new System.Drawing.Size(124, 32);
            this.toolStripButton12.TabIndex = 308;
            this.toolStripButton12.Text = "Buscar Pacientes";
            this.toolStripButton12.UseVisualStyleBackColor = false;
            this.toolStripButton12.Click += new System.EventHandler(this.toolStripButton12_Click);
            // 
            // Contenedor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1037, 642);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label72);
            this.Controls.Add(this.label73);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label78);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.linkLabel16);
            this.Controls.Add(this.linkLabel17);
            this.Controls.Add(this.linkLabel11);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel12);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel7);
            this.Controls.Add(this.linkLabel8);
            this.Controls.Add(this.linkLabel6);
            this.Controls.Add(this.cbVoice);
            this.Controls.Add(this.linkLabel5);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel15);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Contenedor";
            this.Load += new System.EventHandler(this.Contenedor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Contenedor_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox4;
        public System.Windows.Forms.Label label78;
        private System.Windows.Forms.ComboBox cbVoice;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel4;
        public System.Windows.Forms.LinkLabel linkLabel5;
        public System.Windows.Forms.LinkLabel linkLabel6;
        public System.Windows.Forms.LinkLabel linkLabel7;
        public System.Windows.Forms.LinkLabel linkLabel8;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Timer CerrarPanel;
        public System.Windows.Forms.LinkLabel linkLabel11;
        public System.Windows.Forms.LinkLabel linkLabel12;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.LinkLabel linkLabel15;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.LinkLabel linkLabel16;
        public System.Windows.Forms.LinkLabel linkLabel17;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel2;
        private FormAndControls.Controles.Boton toolStripButton1;
        private FormAndControls.Controles.Boton toolStripButton12;
        private FormAndControls.Controles.Boton button1;
    }
}