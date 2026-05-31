using System;
using System.Windows.Forms;
using ZamenisHealth.Medicina;
using ZamenisHealth.Medicina.DocumentosWEB;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Terapia : Form
    {
        public Terapia()
        {
            InitializeComponent();
        }
        private void pictureBox20_Click(object sender, EventArgs e)
        {
            AgendaM M = new AgendaM(false);
            M.ShowDialog();
        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 m = new Medicina.Historial_Medico_1();
            m.ShowDialog();
        }

        private void pictureBox73_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.Historia_JM_Completar_Seleccion HJMS = new HistoriasClinicas.Historia_JM_Completar_Seleccion();
            HJMS.ShowDialog();
        }

        private void pictureBox78_Click(object sender, EventArgs e)
        {
            Fibromialgia.Encuestas.Menu M = new Fibromialgia.Encuestas.Menu();
            M.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Medicina.UploadHistory U = new Medicina.UploadHistory();
            U.ShowDialog();
        }

        private void pictureBox79_Click(object sender, EventArgs e)
        {
            Fibromialgia.Informes.InformesPrograma P = new Fibromialgia.Informes.InformesPrograma();
            P.ShowDialog();
        }

        private void Terapia_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MenuDocWeb M = new MenuDocWeb();
            M.ShowDialog();
        }
    }
}
