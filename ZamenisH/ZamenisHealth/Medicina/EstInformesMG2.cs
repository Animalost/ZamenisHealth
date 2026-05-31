using Domain;
using FormAndControls;
using Persistence;
using Persistence.Informes.Interfaces;
using Persistence.Informes.Methods;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class EstInformesMG2 : Forma2
    {
        private static readonly IInformeEstadistico repoEst = new MInformeEstadistico();

        private int Admision;
        private string Tabla;

        public EstInformesMG2(int admision, string tabla)
        {
            InitializeComponent();
            this.Admision = admision;
            this.Tabla = tabla;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                repoEst.updateTable(this.Tabla, comboBox1.Text, this.Admision);

                EstInformesMG f = Application.OpenForms.OfType<EstInformesMG>().SingleOrDefault();
                f.setCargarGrillas();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void EstInformesMG2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Estadisticas";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }
    }
}
