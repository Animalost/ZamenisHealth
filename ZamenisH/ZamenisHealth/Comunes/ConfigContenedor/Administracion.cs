using Domain;
using Newtonsoft.Json.Linq;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.AdminSystem;
using ZamenisHealth.Consumos;
using ZamenisHealth.Facturacion;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Administracion : Form
    {
        private static readonly IRoles repoRoles = new MRoles();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        public Administracion()
        {
            InitializeComponent();
        }

        private void pictureBox62_Click(object sender, EventArgs e)
        {
            Contenedor f29 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
            f29.panelToAdmin2();
        }
        private void pictureBox63_Click(object sender, EventArgs e)
        {
            Medicina.Cargos C = new Medicina.Cargos(false);
            C.ShowDialog();
        }
        private void pictureBox64_Click(object sender, EventArgs e)
        {            
            RIPS R = new RIPS();
            R.ShowDialog();
        }
        private void pictureBox65_Click(object sender, EventArgs e)
        {
            ConsAdmision f = new ConsAdmision();
            f.ShowDialog();
        }
        private void pictureBox67_Click(object sender, EventArgs e)
        {
            AdminSystem.Adherencia f = new AdminSystem.Adherencia();
            f.ShowDialog();
        }
        private void pictureBox68_Click(object sender, EventArgs e)
        {
            AdminSystem.Formatos formatos = new AdminSystem.Formatos();
            formatos.ShowDialog();
        }
        private void pictureBox69_Click(object sender, EventArgs e)
        {
            if (Conexion.ConectionDictionary["Recordatorios"] == "A")
            {
                Mensajeria.MainMessage mainMessage = new Mensajeria.MainMessage();
                mainMessage.ShowDialog();
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "Su licencia no permite el envio de mensajes de texto o email, contacte al desarrollador";
                MG.ShowDialog();
            }
        }   
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AdminSystem.Autorizaciones A = new AdminSystem.Autorizaciones();
            A.ShowDialog();
        }
        private void Administracion_Load(object sender, EventArgs e)
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

                    JObject obj2 = JObject.Parse(Rol.Administracion);

                    if (obj2["Administracion"] != null)
                    {
                        panel2.Visible = obj2["Administracion"]["Facturacion"] != null ?
                                          obj2["Administracion"]["Facturacion"].ToString() == "A" ? true : false : false;
                        panel12.Visible = obj2["Administracion"]["Anulaciones"] != null ?
                                          obj2["Administracion"]["Anulaciones"].ToString() == "A" ? true : false : false;
                        panel4.Visible = obj2["Administracion"]["Formatos"] != null ?
                                          obj2["Administracion"]["Formatos"].ToString() == "A" ? true : false : false;
                        panel5.Visible = obj2["Administracion"]["Cargos"] != null ?
                                          obj2["Administracion"]["Cargos"].ToString() == "A" ? true : false : false;
                        panel6.Visible = obj2["Administracion"]["Inventarios"] != null ?
                                          obj2["Administracion"]["Inventarios"].ToString() == "A" ? true : false : false;
                        panel7.Visible = obj2["Administracion"]["Mensajero"] != null ?
                                         obj2["Administracion"]["Mensajero"].ToString() == "A" ? true : false : false;
                        panel8.Visible = obj2["Administracion"]["Rips"] != null ?
                                         obj2["Administracion"]["Rips"].ToString() == "A" ? true : false : false;
                        panel9.Visible = obj2["Administracion"]["Adherencia"] != null ?
                                        obj2["Administracion"]["Adherencia"].ToString() == "A" ? true : false : false;
                        panel10.Visible = obj2["Administracion"]["RDA"] != null ?
                                        obj2["Administracion"]["RDA"].ToString() == "A" ? true : false : false;                       
                        panel11.Visible = obj2["Administracion"]["Autorizaciones"] != null ?
                                        obj2["Administracion"]["Autorizaciones"].ToString() == "A" ? true : false : false;
                        panel13.Visible = obj2["Administracion"]["RDA"] != null ?
                                        obj2["Administracion"]["RDA"].ToString() == "A" ? true : false : false;
                    }
                    else
                    {
                        flowLayoutPanel1.Visible = false;
                    }
                }
                if (repoConfSystem.getListado()["IHCE"] == "A")
                {
                    panel10.Visible = true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Facturacion.Extras.ConsAutorizacion a = new Facturacion.Extras.ConsAutorizacion();
            a.ShowDialog();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Asignacion P = new Asignacion();
            P.ShowDialog();
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            FrontFHIR.EnvioFHIR f = new FrontFHIR.EnvioFHIR();
            f.ShowDialog();
        }
        private void label6_Click(object sender, EventArgs e)
        {
            BarCodesForm F = new BarCodesForm();
            F.ShowDialog();
        }
    }
}
