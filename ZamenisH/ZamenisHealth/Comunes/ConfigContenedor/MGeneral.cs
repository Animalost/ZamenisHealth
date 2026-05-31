using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.HistoriasClinicas;
using ZamenisHealth.HistoriasClinicas.Extras;
using ZamenisHealth.Medicina;
using ZamenisHealth.Medicina.DocumentosWEB;
using ZamenisHealth.Medicina.OrdenesExtra;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class MGeneral : Form
    {
        private static readonly ILogin repositorioLogin = new MLogin();
        private static readonly IConfSystem repositorioConf = new MConfSystem();
        private static readonly IBodegas repositorioBodegas = new MBodegas();

        private MensajesGeneral MG;

        public MGeneral()
        {
            InitializeComponent();
        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {
            AgendaM M = new AgendaM(true);
            M.ShowDialog();
        }

        private void pictureBox24_Click(object sender, EventArgs e)
        {
            Medicina.CambioManejoMedico M = new Medicina.CambioManejoMedico();
            M.ShowDialog();
        }

        private void pictureBox25_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.NotasAclaratorias NA = new HistoriasClinicas.NotasAclaratorias();
            NA.TipoNota = "MedGen";
            NA.ShowDialog();
        }

        private void pictureBox26_Click(object sender, EventArgs e)
        {
            CrearOrdenExtra crearOrdenExtra = new CrearOrdenExtra("MG");
            crearOrdenExtra.ShowDialog();
            /*Extras.TipoOrden T = new Extras.TipoOrden("MG");
            T.ShowDialog();*/
        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {
            Medicina.RetomarHC D = new Medicina.RetomarHC();
            D.Tipo = "MG";
            D.ShowDialog();
        }

        private void pictureBox28_Click(object sender, EventArgs e)
        {

            Medicina.RegImagenes RI = new Medicina.RegImagenes();
            RI.ShowDialog();
        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {
            Medicina.VerImagenes VI = new Medicina.VerImagenes();
            VI.ShowDialog();
        }

        private void pictureBox30_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 m = new Medicina.Historial_Medico_1();
            m.ShowDialog();
        }

        private void pictureBox76_Click(object sender, EventArgs e)
        {
            CXN_BODEGAS get = repositorioBodegas.getDatosUser(Contenedor.UsuarioLogueado);
            if (get == null)
            {
                MG = new MensajesGeneral();
                MG.Mensaje = "Su usuario no es tipo medico o enfermero";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
            }
            else
            {
                INV.Consultorios.VerInventario I = new INV.Consultorios.VerInventario(get.Bod_Numero, true);
                I.ShowDialog();
            }            
        }

        private void MGeneral_Load(object sender, EventArgs e)
        {
            try
            {
                var imageJefe = repositorioLogin.getUser(Contenedor.UsuarioLogueado);
                if (imageJefe == null)
                {
                    pictureBox28.Visible = false;
                    label44.Visible = false;
                }
                else
                {
                    if (imageJefe.Log_Fotos == "A")
                    {
                        pictureBox28.Visible = true;
                        label44.Visible = true;
                    }
                    else
                    {
                        pictureBox28.Visible = false;
                        label44.Visible = false;
                    }                 
                }

                if (repositorioConf.getListado()["ConsentimientosWEB"] != "A")
                {
                    pictureBox3.Visible = false;
                    label4.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Medicina.EstInformesMG E = new Medicina.EstInformesMG();
            E.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Medicina.UploadHistory U = new Medicina.UploadHistory();
            U.ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MenuDocWeb M = new MenuDocWeb();
            M.ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            AprobarSolicitudes A = new AprobarSolicitudes();
            A.ShowDialog();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Historia_MedicinaGeneral_2_2  hMG = new Historia_MedicinaGeneral_2_2();
            hMG.ShowDialog();
        }
    }
}
