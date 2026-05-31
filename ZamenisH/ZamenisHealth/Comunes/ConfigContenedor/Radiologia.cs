using System;
using System.Windows.Forms;
using ZamenisHealth.Medicina;
using ZamenisHealth.Medicina.OrdenesExtra;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Radiologia : Form
    {
        public Radiologia()
        {
            InitializeComponent();
        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {
            Medicina.VerImagenes VI = new Medicina.VerImagenes();
            VI.ShowDialog();
        }

        private void pictureBox30_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 m = new Medicina.Historial_Medico_1();
            m.ShowDialog();
        }

        private void pictureBox28_Click(object sender, EventArgs e)
        {
            Medicina.RegImagenes RI = new Medicina.RegImagenes();
            RI.ShowDialog();
        }

        private void pictureBox26_Click(object sender, EventArgs e)
        {
            CrearOrdenExtra crearOrdenExtra = new CrearOrdenExtra("RA");
            crearOrdenExtra.ShowDialog();
        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {
            AgendaM M = new AgendaM(false);
            M.ShowDialog();
        }

        private void pictureBox25_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.NotasAclaratorias NA = new HistoriasClinicas.NotasAclaratorias();
            NA.TipoNota = "Radiologia";
            NA.ShowDialog();
        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {
            Medicina.RetomarHC D = new Medicina.RetomarHC();
            D.Tipo = "RA";
            D.ShowDialog();
        }
    }
}
