using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Medicina;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class AprobarSolicitudes2 : Forma
    {
        private readonly ICambiosSolicitados repoCambios = new MCambiosSolicitados();
        private readonly IAgendaC repoAgenda = new MAgendaC();
        private readonly ICIE10 repoCie10 = new MCIE10();
        private readonly ICompañia repoCia = new MCompañia();
        private readonly IOrdenes repositorioOrdenes = new MOrdenes();

        private MensajesGeneral MG;
        private int Posision, Admision;

        ToolStripButton btnAprobar, btnDenegar;

        public AprobarSolicitudes2(int posision, int admision)
        {
            InitializeComponent();
            Posision = posision;
            Admision = admision;
        }

        private void AprobarSolicitudes2_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Solicitudes";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                btnAprobar = new ToolStripButton();
                btnAprobar = createToolButton("Aprobar");
                MenuLateral.Items.Add(btnAprobar);
                btnAprobar.Click += button1_Click;

                btnDenegar = new ToolStripButton();
                btnDenegar = createToolButton("Denegar");
                MenuLateral.Items.Add(btnDenegar);
                btnDenegar.Click += button2_Click;

                CXN_CAMBIOSSOLICITADOS C = repoCambios.GetSolicitud(Posision);
                if (C == null)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro cargar esta solicitud";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    textBox1.Text = C.CupComplejidad + " - " + C.ServComplejidad;
                    richTextBox1.Text = C.Motivo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea aprobar esta nueva orden medica?",
                                                "Zamenis Health - Aprobar solicitud de ordenes medicas",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    repoCambios.CambiarEstadoSolicitud(Posision, "A");

                    AprobarSolicitudes f1 = Application.OpenForms.OfType<AprobarSolicitudes>().SingleOrDefault();
                    f1.setCargarSolicitudes();

                    otrosDatosPacienteHorario o = repoAgenda.cargarAdmision(Admision, "'P','H','A'");
                    if (o != null)
                    {
                        string Dx1 = "L97X";

                        (string dx1, string dx2, string dx3) dx = repoCambios.getDX(Admision);
                        CXN_CIA compData = repoCia.getPrestadorbyCode(o.Hor_Pac_Cia);

                        CXN_OM OM = new CXN_OM
                        {
                            OM_Pac = o.Hor_Pac_Id,
                            OM_Ase = o.Hor_Pac_Ase,
                            OM_Cia = o.Hor_Pac_Cia,
                            OM_Prof = Comunes.Contenedor.UsuarioLogueado,
                            OM_Desc = "Se solicitan 8 curaciones de " + textBox1.Text,
                            OM_DX1 = string.IsNullOrEmpty(dx.dx1) ? Dx1 : dx.dx1,
                            OM_DX2 = string.IsNullOrEmpty(dx.dx2) ? "" : dx.dx2,
                            OM_DX3 = string.IsNullOrEmpty(dx.dx2) ? "" : dx.dx3,
                            OM_DX1T = string.IsNullOrEmpty(dx.dx1) ? repoCie10.BuscaDX(Dx1) : repoCie10.BuscaDX(dx.dx1),
                            OM_DX2T = string.IsNullOrEmpty(dx.dx2) ? "" : repoCie10.BuscaDX(dx.dx2),
                            OM_DX3T = string.IsNullOrEmpty(dx.dx2) ? "" : repoCie10.BuscaDX(dx.dx3),
                            OM_Edad = "",
                            OM_Genero = string.IsNullOrEmpty(o.Pac_Sexo) ? "Indeterminado" : o.Pac_Sexo == "I" ? "Indeterminado" : o.Pac_Sexo == "M" ? "Masculino" : "Femenino",
                            OM_Direccion = o.Pac_DireccionLoadAdmition,
                            OM_Telefono = o.Pac_Telefono,
                            OM_Num = compData.Com_OM,
                            OM_Firma = "SI",
                            OM_TEspecialidad = "MG",
                            OM_Clasificacion = "ORDEN DE SERVICIOS",
                            OM_Planillar = "S"
                        };

                        bool grabarOrden = repositorioOrdenes.CrearOrden(OM);
                        if (grabarOrden != true)
                        {
                            MessageBox.Show("No se logro generar la orden medica",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);                           
                        }
                        else
                        {
                            int NuevoNumero = Convert.ToInt32(OM.OM_Num) + 1;
                            bool _updateCons = repoCia.ConsecutivoActualiza(OM.OM_Cia, "OM", NuevoNumero);

                            var Exportar = repositorioOrdenes.Generar_OrdenMedica(Convert.ToInt32(compData.Com_OM), compData.Com_Identificador, Comunes.Contenedor.UsuarioLogueado);
                            if (Exportar == null)
                            {
                                MessageBox.Show("La orden medica se genero pero no se logro exportar, ingrese por la opcion " +
                                    "de busqueda de ordenes medicas",
                                    "Inconsistencia",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                            }
                            else
                            {
                                OrdenesMedicasT OT = new OrdenesMedicasT(compData.Com_Identificador,
                                                            Convert.ToInt32(compData.Com_OM),
                                                            Comunes.Contenedor.UsuarioLogueado);

                                OT.ShowDialog();
                            }
                        }

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Error desconocido";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }                        
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea denegar esta nueva orden medica?",
                                               "Zamenis Health - Denegar solicitud de ordenes medicas",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                repoCambios.CambiarEstadoSolicitud(Posision, "D");

                AprobarSolicitudes f1 = Application.OpenForms.OfType<AprobarSolicitudes>().SingleOrDefault();
                f1.setCargarSolicitudes();

                this.Dispose();
                this.Close();
            }
        }
    }
}
