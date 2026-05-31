using FormAndControls;

using Persistence;

using System;

namespace ZamenisHealth.Comunes
{
    public partial class MensajesGeneral : Forma2
    {
        // 0 CrearEditarPaciente, AgendarCita
        // 1 Claves
        // 2 Agenda - SMS
        // 3 Acuerdo
        public string Mensaje;
        public int TipoImagen;

        public MensajesGeneral()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void MensajesGeneral_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Mensaje Zamenis Health";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            ImageClose.Visible = false;
            ImageMinimize.Visible = false;

            this.TopMost = true;

            label2.Text = Mensaje;

            switch (TipoImagen)
            {
                case 0:
                    pictureBox1.Image = Properties.Resources2.alerta;
                    break;

                case 1:
                    pictureBox1.Image = Properties.Resources2.keys;
                    break;

                case 2:
                    pictureBox1.Image = Properties.Resources2.Cell_Black;
                    break;

                case 3:
                    pictureBox1.Image = Properties.Resources2.comprobado;
                    break;

                default:
                    pictureBox1.Image = Properties.Resources2.cerca;
                    break;
            }

            this.BringToFront();
        }
    }
}
