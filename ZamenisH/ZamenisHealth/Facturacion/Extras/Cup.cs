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
    public partial class Cup : Forma
    {
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IAgendaC repoAgenda = new MAgendaC();

        private int Admision, Posision;
        public Cup(int admision, int Posision)
        {
            InitializeComponent();
            this.Admision = admision;
            this.Posision = Posision;

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
                var getDatosCargo = repoCargos.getValores(this.Posision);
                if (getDatosCargo != null)
                {
                    var getNuevosDatos = repoConvenios.DatosServicioXNameAse(getDatosCargo.Car_Ase, comboBox1.Text);
                    if (getNuevosDatos != null)
                    {
                        CXN_CARGOS C = new CXN_CARGOS
                        {
                            Car_Cod = getNuevosDatos.Con_Id_Serv,
                            Car_Val_Un = getNuevosDatos.Con_Valor,
                            Car_Val_Tot = getNuevosDatos.Con_Valor,
                            Car_Item = getNuevosDatos.Con_Nombre,
                            Car_Tipo_Serv = getNuevosDatos.Con_Tipo_Serv,
                            Car_Id = this.Posision
                        };

                        bool updateDatosCargo = repoCargos.updateCargos(C);
                        if (updateDatosCargo != true)
                        {
                            MensajesGeneral MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Hubo un error inesperado";
                            MG.ShowDialog();
                        }
                        else
                        {
                            repoAgenda.updateServicoFromFactura(Convert.ToInt32(label2.Text), C.Car_Cod);

                            MensajesGeneral MG = new MensajesGeneral();
                            MG.TipoImagen = 3;
                            MG.Mensaje = "Hecho";
                            MG.ShowDialog();
                            Cerrar();
                        }
                    }
                    else
                    {
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Hubo un error inesperado";
                        MG.ShowDialog();
                    }
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Hubo un error inesperado";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Cup_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Cambio de CUPS";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button1_Click;

                comboBox1.Items.Clear();

                var getLista = repoConvenios.CargarCUPS(this.Posision);
                if (getLista != null)
                {
                    foreach (var i in getLista)
                    {
                        comboBox1.Items.Add(i);
                    }
                }
                else
                {
                    comboBox1.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error #", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
