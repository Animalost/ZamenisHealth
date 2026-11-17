using FormAndControls;
using Persistence;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ZamenisHealth.Medicina
{
    public partial class CIE11 : Forma2
    {
        private string Codigo, Diagnostico, Definicion;

        private void boton2_Click(object sender, EventArgs e)
        {
            CIE11New f1 = Application.OpenForms.OfType<CIE11New>().SingleOrDefault();
            f1.selectedCIE11 = false;

            this.Close();
        }

        private void boton1_Click(object sender, EventArgs e)
        {
            CIE11New f1 = Application.OpenForms.OfType<CIE11New>().SingleOrDefault();
            f1.selectedCIE11 = true;

            this.Close();
        }

        public CIE11(string codigo, string diagnostico, string definicion)
        {
            InitializeComponent();
            Codigo = codigo;
            Diagnostico = diagnostico;
            Definicion = definicion;            
        }

        private void CIE11_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Descripcion del Diagnostico";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            label1.Text = Codigo + " - " + Diagnostico;
            textBox1.Text = Definicion;

            ImageClose.Visible = false;
        }
    }
}
