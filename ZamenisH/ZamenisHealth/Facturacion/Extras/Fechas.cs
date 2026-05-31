using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion.Extras
{
    public partial class Fechas : Forma
    {
        private static readonly ICargos repoCargos = new MCargos();

        private int Admision, Posision;

        public Fechas(int admision, int posision)
        {
            InitializeComponent();
            this.Admision = admision;
            this.Posision = posision;

            this.label2.Text = this.Admision.ToString();
            this.label3.Text = this.Posision.ToString();
        }
        void Cerrar()
        {
            this.Dispose();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bool _updateF = repoCargos.updateDate(Convert.ToDateTime(dateTimePicker1.Value.Date), this.Posision);
                if (_updateF != true)
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Hubo un error inesperado";
                    MG.ShowDialog();
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Hecho";
                    MG.ShowDialog();

                    Cerrar();
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Fechas_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cambo de Fecha de Servicio";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;

        }
    }
}
