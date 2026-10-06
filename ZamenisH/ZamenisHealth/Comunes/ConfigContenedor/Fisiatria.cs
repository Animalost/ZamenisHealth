using Domain;
using Newtonsoft.Json.Linq;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Medicina;
using ZamenisHealth.Medicina.OrdenesExtra;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Fisiatria : Form
    {
        private IRoles repoRoles = new MRoles();
        public Fisiatria()
        {
            InitializeComponent();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            AgendaM M = new AgendaM(false);
            M.ShowDialog();
        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.NotasAclaratorias NA = new HistoriasClinicas.NotasAclaratorias();
            NA.TipoNota = "Fisiatria";
            NA.ShowDialog();
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            CrearOrdenExtra crearOrdenExtra = new CrearOrdenExtra("FI");
            crearOrdenExtra.ShowDialog();

            /*Extras.TipoOrden T = new Extras.TipoOrden("FI");
            T.ShowDialog();*/
        }
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Medicina.RetomarHC D = new Medicina.RetomarHC();
            D.Tipo = "FI";
            D.ShowDialog();
        }
        private void pictureBox74_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.Historia_JM_Completar_Seleccion HJMS = new HistoriasClinicas.Historia_JM_Completar_Seleccion();
            HJMS.ShowDialog();
        }
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 m = new Medicina.Historial_Medico_1();
            m.ShowDialog();
        }
        private void pictureBox71_Click(object sender, EventArgs e)
        {
            Medicina.FirmaHistorias firmaHistorias = new Medicina.FirmaHistorias();
            firmaHistorias.ShowDialog();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Medicina.UploadHistory U = new Medicina.UploadHistory();
            U.ShowDialog();
        }
        private void Fisiatria_Load(object sender, EventArgs e)
        {
            CXN_DESKTOP_ROLES Rol = repoRoles.getDesktopRoles(Contenedor.UsuarioLogueado);
            if (Rol == null)
            {
                flowLayoutPanel1.Visible = false;
            }
            else
            {
                flowLayoutPanel1.Visible = true;

                JObject obj7 = JObject.Parse(Rol.Fisiatria);

                if (obj7["Fisiatria"] != null)
                {
                    panel2.Visible = obj7["Fisiatria"]["CrearHistoria"] != null ?
                                        obj7["Fisiatria"]["CrearHistoria"].ToString() == "A" ? true : false : false;
                    panel3.Visible = obj7["Fisiatria"]["Retomar"] != null ?
                                       obj7["Fisiatria"]["Retomar"].ToString() == "A" ? true : false : false;
                    panel6.Visible = obj7["Fisiatria"]["SubirHistoria"] != null ?
                                      obj7["Fisiatria"]["SubirHistoria"].ToString() == "A" ? true : false : false;
                    panel7.Visible = obj7["Fisiatria"]["NotaAclaratoria"] != null ?
                                      obj7["Fisiatria"]["NotaAclaratoria"].ToString() == "A" ? true : false : false;
                    panel4.Visible = obj7["Fisiatria"]["CompletarJuntas"] != null ?
                                      obj7["Fisiatria"]["CompletarJuntas"].ToString() == "A" ? true : false : false;
                    panel5.Visible = obj7["Fisiatria"]["FirmarHistorias"] != null ?
                                      obj7["Fisiatria"]["FirmarHistorias"].ToString() == "A" ? true : false : false;
                    panel8.Visible = obj7["Fisiatria"]["CrearOrdenes"] != null ?
                                      obj7["Fisiatria"]["CrearOrdenes"].ToString() == "A" ? true : false : false;
                    panel9.Visible = obj7["Fisiatria"]["BuscarRegistros"] != null ?
                                      obj7["Fisiatria"]["BuscarRegistros"].ToString() == "A" ? true : false : false;
                }
                else
                {
                    flowLayoutPanel1.Visible = false;
                }
            }
        }
    }
}
