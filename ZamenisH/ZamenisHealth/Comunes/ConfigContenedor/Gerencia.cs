using Domain;
using Newtonsoft.Json.Linq;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Gerenciales;
using ZamenisHealth.HistoriasClinicas;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Gerencia : Form
    {
        private IRoles repoRoles = new MRoles();
        private ICruces oController = new MCruces();
        private MensajesGeneral MG;

        public Gerencia()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Gerencia ver = new ZamenisHealth.Gerencia();
            ver.ShowDialog();
        }
        private void label2_Click(object sender, EventArgs e)
        {
            ReportesGerenciales reportesGerenciales = new ReportesGerenciales();
            reportesGerenciales.ShowDialog();
        }
        private void label6_Click(object sender, EventArgs e)
        {
            Historia_MedicinaGeneral_2_2 hMG = new Historia_MedicinaGeneral_2_2();
            hMG.ShowDialog();
        }
        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                string CargoDigitado = Microsoft.VisualBasic.Interaction.InputBox(
                                                "Digite el numero de cierre de caja.  Esta accion no se puede deshacer",
                                                "Eliminar Cierre de Caja",
                                                    "");
                if (CargoDigitado != "")
                {
                    oController.EliminaCierre(Convert.ToInt32(CargoDigitado));

                    MG = new MensajesGeneral
                    {
                        Mensaje = "Cierre de caja eliminado correctamente",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Grave", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
        private void Gerencia_Load(object sender, EventArgs e)
        {
            CXN_DESKTOP_ROLES Rol = repoRoles.getDesktopRoles(Contenedor.UsuarioLogueado);
            if (Rol == null)
            {
                flowLayoutPanel1.Visible = false;
            }
            else
            {
                flowLayoutPanel1.Visible = true;
                JObject obj6 = JObject.Parse(Rol.Gerencial);

                if (obj6["Gerencial"] != null)
                {
                    panel2.Visible = obj6["Gerencial"]["FacturaPaciente"] != null ?
                                        obj6["Gerencial"]["FacturaPaciente"].ToString() == "A" ? true : false : false;
                    panel3.Visible = obj6["Gerencial"]["Reportes"] != null ?
                                       obj6["Gerencial"]["Reportes"].ToString() == "A" ? true : false : false;
                    panel6.Visible = obj6["Gerencial"]["Consentimientos"] != null ?
                                      obj6["Gerencial"]["Consentimientos"].ToString() == "A" ? true : false : false;
                    panel7.Visible = obj6["Gerencial"]["EliminarCierreCaja"] != null ?
                                      obj6["Gerencial"]["EliminarCierreCaja"].ToString() == "A" ? true : false : false;
                }
                else
                {
                    flowLayoutPanel1.Visible = false;
                }
            }
        }
    }
}
 