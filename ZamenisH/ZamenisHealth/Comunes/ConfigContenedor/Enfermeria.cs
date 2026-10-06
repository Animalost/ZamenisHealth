using Domain;
using Domain.CXN;
using Newtonsoft.Json.Linq;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Medicina.DocumentosWEB;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Enfermeria : Form
    {
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly IRoles repoRoles = new MRoles();

        private MensajesGeneral MG;

        public Enfermeria()
        {
            InitializeComponent();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {           
            Medicina.AgendaM M = new Medicina.AgendaM(false);
            M.ShowDialog();
        }
        private void pictureBox12_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.NotasAclaratorias NA = new HistoriasClinicas.NotasAclaratorias();
            NA.TipoNota = "Curaciones";
            NA.ShowDialog();
        }
        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Medicina.CambioManejoEnfermero CME = new Medicina.CambioManejoEnfermero();
            CME.ShowDialog();
        }
        private void pictureBox14_Click(object sender, EventArgs e)
        {
            Medicina.Cargos M = (new Medicina.Cargos(false));
            M.ShowDialog();
        }
        private void pictureBox15_Click(object sender, EventArgs e)
        {
            Medicina.PlantillasEnfermeria Historia_Notas_Plantilla = new Medicina.PlantillasEnfermeria();
            Historia_Notas_Plantilla.dataGridView1.Enabled = false;
            Historia_Notas_Plantilla.ShowDialog();
        }
        private void pictureBox16_Click(object sender, EventArgs e)
        {
            Medicina.VerImagenes VI = new Medicina.VerImagenes();
            VI.ShowDialog();
        }
        private void pictureBox17_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 m = new Medicina.Historial_Medico_1();
            m.ShowDialog();
        }
        private void pictureBox36_Click(object sender, EventArgs e)
        {
            Medicina.RegImagenes RI = new Medicina.RegImagenes();
            RI.ShowDialog();
        }
        private void pictureBox75_Click(object sender, EventArgs e)
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
        private void Enfermeria_Load(object sender, EventArgs e)
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

                    JObject obj4 = JObject.Parse(Rol.Enfermeria);

                    if (obj4["Enfermeria"] != null)
                    {
                        panel2.Visible = obj4["Enfermeria"]["Notas"] != null ?
                                            obj4["Enfermeria"]["Notas"].ToString() == "A" ? true : false : false;
                        panel3.Visible = obj4["Enfermeria"]["Plantillas"] != null ?
                                           obj4["Enfermeria"]["Plantillas"].ToString() == "A" ? true : false : false;
                        panel4.Visible = obj4["Enfermeria"]["Imagenes"] != null ?
                                          obj4["Enfermeria"]["Imagenes"].ToString() == "A" ? true : false : false;
                        panel5.Visible = obj4["Enfermeria"]["NAclaratoria"] != null ?
                                          obj4["Enfermeria"]["NAclaratoria"].ToString() == "A" ? true : false : false;
                        panel6.Visible = obj4["Enfermeria"]["SearchImages"] != null ?
                                          obj4["Enfermeria"]["SearchImages"].ToString() == "A" ? true : false : false;
                        panel7.Visible = obj4["Enfermeria"]["Inventario"] != null ?
                                         obj4["Enfermeria"]["Inventario"].ToString() == "A" ? true : false : false;
                        panel8.Visible = obj4["Enfermeria"]["CManejo"] != null ?
                                         obj4["Enfermeria"]["CManejo"].ToString() == "A" ? true : false : false;
                        panel9.Visible = obj4["Enfermeria"]["Registros"] != null ?
                                        obj4["Enfermeria"]["Registros"].ToString() == "A" ? true : false : false;
                        panel10.Visible = obj4["Enfermeria"]["Estadisticas"] != null ?
                                      obj4["Enfermeria"]["Estadisticas"].ToString() == "A" ? true : false : false;
                        panel11.Visible = obj4["Enfermeria"]["Cargos"] != null ?
                                      obj4["Enfermeria"]["Cargos"].ToString() == "A" ? true : false : false;
                        panel12.Visible = obj4["Enfermeria"]["SubirDocumentos"] != null ?
                                      obj4["Enfermeria"]["SubirDocumentos"].ToString() == "A" ? true : false : false;
                        panel13.Visible = obj4["Enfermeria"]["DocumentosWEB"] != null ?
                                     obj4["Enfermeria"]["DocumentosWEB"].ToString() == "A" ? true : false : false;
                    }
                    else
                    {
                        flowLayoutPanel1.Visible = false;
                    }
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
    }
}
