using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth
{
    public partial class Inicial : ConfigForm.BaseForm
    {
        public Inicial()
        {
            InitializeComponent();            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Inicio I = new Inicio();
            I.ShowDialog();
        }

        private void Inicial_Load(object sender, EventArgs e)
        {
            Titulo.Visible = false;
            ImageClose.Visible = false;

            AbrirFormEnPanel(new Comunes.Extras.InitializeClock());
            label4.Text = "Version: " + Application.ProductVersion.ToString();

            ToolTip T1 = new ToolTip();
            ToolTip T2 = new ToolTip();
            
            T1.ShowAlways = true;
            T1.SetToolTip(button1, "Abrir aplicacion Zamenis Health, sistema de historias clinicas");
        }

        private void AbrirFormEnPanel(object Formhijo)
        {
            if (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(fh);
            this.panel1.Tag = fh;
            panel1.Visible = true;
            fh.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
