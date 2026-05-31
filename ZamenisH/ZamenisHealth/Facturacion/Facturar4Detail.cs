using Domain;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class Facturar4Detail : Forma2
    {
        private static readonly IFacturacion repoFc = new MFacturacion();

        int SumasCantidad, SumasTotal;
        public Facturar4Detail()
        {
            InitializeComponent();
        }

        private void Facturar4Detail_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Detallado de Documentos";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar un numero de documento valido"); return; }

            try
            {
                var getExport = repoFc.getDetailDE(Convert.ToInt32(textBox1.Text));
                if (getExport != null)
                {
                    ConfigForm.GenerarReportViewer("DataSet_DetailDE",
                                             "ZamenisHealth.Reportes.RDLC_DetalleDE.rdlc",
                                             getExport);
                }
                else
                {
                    MessageBox.Show("No se logro encontrar el detalle del presente documento equivalente",
                            "Sin Resultados",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
