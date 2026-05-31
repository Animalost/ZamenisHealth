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
    public partial class Estado : Forma
    {
        private static readonly ICargos repoCargos = new MCargos();

        private int Admision, Posision;

        public Estado(int admision, int posision)
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
        private void Estado_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cambio de Estado Admision";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string Est;

                switch (comboBox1.Text)
                {
                    case "Habilitar Cargo":
                        Est = "G";
                        break;

                    case "Deshabilitar Cargo":
                        Est = "A";
                        break;

                    default:
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.Mensaje = "Eleccion no valida";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                }

                bool _updateHab = repoCargos.HabilitaInhabilita(Est, this.Posision);
                if (_updateHab != true)
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = "Hubo un error inesperado";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = "Hecho";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();

                    Cerrar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
