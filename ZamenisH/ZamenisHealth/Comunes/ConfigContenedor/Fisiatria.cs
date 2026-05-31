using System;
using System.Windows.Forms;
using ZamenisHealth.Medicina;
using ZamenisHealth.Medicina.OrdenesExtra;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Fisiatria : Form
    {
        public Fisiatria()
        {
            InitializeComponent();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            AgendaM M = new AgendaM(false);
            M.ShowDialog();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.NotasAclaratorias NA = new HistoriasClinicas.NotasAclaratorias();
            NA.TipoNota = "Fisiatria";
            NA.ShowDialog();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            CrearOrdenExtra crearOrdenExtra = new CrearOrdenExtra("FI");
            crearOrdenExtra.ShowDialog();

            /*Extras.TipoOrden T = new Extras.TipoOrden("FI");
            T.ShowDialog();*/
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Medicina.RetomarHC D = new Medicina.RetomarHC();
            D.Tipo = "FI";
            D.ShowDialog();
        }

        private void pictureBox74_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.Historia_JM_Completar_Seleccion HJMS = new HistoriasClinicas.Historia_JM_Completar_Seleccion();
            HJMS.ShowDialog();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 m = new Medicina.Historial_Medico_1();
            m.ShowDialog();
        }

        private void pictureBox71_Click(object sender, EventArgs e)
        {
            Medicina.FirmaHistorias firmaHistorias = new Medicina.FirmaHistorias();
            firmaHistorias.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Medicina.UploadHistory U = new Medicina.UploadHistory();
            U.ShowDialog();
        }
    }
}
