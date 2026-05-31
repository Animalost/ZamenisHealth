using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes.Extras;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void pictureBox33_Click(object sender, EventArgs e)
        {
            Contenedor f29 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
            f29.Claves();
        }

        private void pictureBox34_Click(object sender, EventArgs e)
        {
            Licence.Acuerdo L = new Licence.Acuerdo();
            L.ShowDialog();
        }

        private void pictureBox35_Click(object sender, EventArgs e)
        {
            UnificadorPDF f = new UnificadorPDF();
            f.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            CompresorZIP f = new CompresorZIP();
            f.ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MisDatos m = new MisDatos();
            m.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                this.Invoke((Action)(() =>
                {
                    Tipificador.Ingreso ingreso = new Tipificador.Ingreso();
                    ingreso.Show();
                }));
            });            
        }
    }
}
