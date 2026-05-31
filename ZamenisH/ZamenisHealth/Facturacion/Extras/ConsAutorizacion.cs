using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion.Extras
{
    public partial class ConsAutorizacion : Forma
    {
        private static readonly IFacturacion repoFac = new MFacturacion();

        public ConsAutorizacion()
        {
            InitializeComponent(); 
            
        }

        private void ConsAutorizacion_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Consultar Autorizacion";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Consulta();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void ConsAutorizacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Consulta();
            }
        }
        void Consulta()
        {
            try
            {
                CXN_FACTURA getAut = repoFac.consAutorizacion(textBox1.Text);
                if (getAut != null)
                {
                    label2.Text = "Esta autorizacion YA ha sido consumida en sistema \n\r \n\r" +
                        "Numero de Factura Zamenis: " + getAut.Fac_Num_Fac + "\n\r" +
                        "Numero Homolgo Electronico: " + getAut.Homologo + "\n\r" +
                        "Fecha de Factura: " + Convert.ToDateTime(getAut.Fac_Fecha).ToString("dd-MM-yyyy") + "\n\r" +
                        "Fecha Desde: " + Convert.ToDateTime(getAut.Fac_Fecha_Des).ToString("dd-MM-yyyy") + "\n\r" +
                        "Fecha Hasta: " + Convert.ToDateTime(getAut.Fac_Fecha_Has).ToString("dd-MM-yyyy");
                }
                else
                {
                    label2.Text = "Esta autorizacion no ha sido consumida en sistema";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
