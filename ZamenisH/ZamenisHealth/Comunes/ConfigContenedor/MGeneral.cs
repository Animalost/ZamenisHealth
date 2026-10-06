using Domain;
using Domain.CXN;
using Newtonsoft.Json.Linq;
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
        private static readonly IRoles repoRoles = new MRoles();
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
                Consumos.Movimientos I = new Consumos.Movimientos();
                I.ShowDialog();
            }            
        }
        private void MGeneral_Load(object sender, EventArgs e)
        {
            try
            {
                CXN_DESKTOP_ROLES Rol = repoRoles.getDesktopRoles(Contenedor.UsuarioLogueado);
                if (Rol == null)
                {
                    flowLayoutPanel1.Visible = false;
                }
                else
                {
                    flowLayoutPanel1.Visible = true;

                    JObject obj5 = JObject.Parse(Rol.MedicinaGeneral);

                    if (obj5["MedicinaGeneral"] != null)
                    {
                        panel2.Visible = obj5["MedicinaGeneral"]["Historia"] != null ?
                                            obj5["MedicinaGeneral"]["Historia"].ToString() == "A" ? true : false : false;
                        panel3.Visible = obj5["MedicinaGeneral"]["Registros"] != null ?
                                           obj5["MedicinaGeneral"]["Registros"].ToString() == "A" ? true : false : false;
                        panel4.Visible = obj5["MedicinaGeneral"]["Retomar"] != null ?
                                          obj5["MedicinaGeneral"]["Retomar"].ToString() == "A" ? true : false : false;
                        panel5.Visible = obj5["MedicinaGeneral"]["CManejo"] != null ?
                                          obj5["MedicinaGeneral"]["CManejo"].ToString() == "A" ? true : false : false;
                        panel6.Visible = obj5["MedicinaGeneral"]["Subir"] != null ?
                                          obj5["MedicinaGeneral"]["Subir"].ToString() == "A" ? true : false : false;
                        panel7.Visible = obj5["MedicinaGeneral"]["Estadistica"] != null ?
                                         obj5["MedicinaGeneral"]["Estadistica"].ToString() == "A" ? true : false : false;
                        panel8.Visible = obj5["MedicinaGeneral"]["Nota"] != null ?
                                         obj5["MedicinaGeneral"]["Nota"].ToString() == "A" ? true : false : false;
                        panel9.Visible = obj5["MedicinaGeneral"]["Inventario"] != null ?
                                        obj5["MedicinaGeneral"]["Inventario"].ToString() == "A" ? true : false : false;
                        panel10.Visible = obj5["MedicinaGeneral"]["DocumentosWEB"] != null ?
                                      obj5["MedicinaGeneral"]["DocumentosWEB"].ToString() == "A" ? true : false : false;
                        panel11.Visible = obj5["MedicinaGeneral"]["GrabarImagenes"] != null ?
                                      obj5["MedicinaGeneral"]["GrabarImagenes"].ToString() == "A" ? true : false : false;
                        panel12.Visible = obj5["MedicinaGeneral"]["Ordenes"] != null ?
                                      obj5["MedicinaGeneral"]["Ordenes"].ToString() == "A" ? true : false : false;
                        panel13.Visible = obj5["MedicinaGeneral"]["Solicitudes"] != null ?
                                     obj5["MedicinaGeneral"]["Solicitudes"].ToString() == "A" ? true : false : false;                       
                        panel14.Visible = obj5["MedicinaGeneral"]["BuscarImagenes"] != null ?
                                     obj5["MedicinaGeneral"]["BuscarImagenes"].ToString() == "A" ? true : false : false;
                        panel15.Visible = obj5["MedicinaGeneral"]["Consentimientos"] != null ?
                                     obj5["MedicinaGeneral"]["Consentimientos"].ToString() == "A" ? true : false : false;
                        panel16.Visible = obj5["MedicinaGeneral"]["Salidas"] != null ?
                                   obj5["MedicinaGeneral"]["Salidas"].ToString() == "A" ? true : false : false;
                    }
                    else
                    {
                        flowLayoutPanel1.Visible = false;
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
        private void label7_Click(object sender, EventArgs e)
        {
            AprobarSalidas A = new AprobarSalidas();
            A.ShowDialog();
        }
    }
}
