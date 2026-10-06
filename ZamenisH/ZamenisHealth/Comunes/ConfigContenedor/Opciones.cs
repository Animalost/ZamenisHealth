using Domain;
using Newtonsoft.Json.Linq;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.AdminSystem;
using ZamenisHealth.AdminSystem.Profesionales;
using ZamenisHealth.Comunes.Extras;
using ZamenisHealth.PagosApp;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Opciones : Form
    {
        private static readonly IRoles repoRoles = new MRoles();

        public Opciones()
        {
            InitializeComponent();
        }
      
        private void pictureBox38_Click(object sender, EventArgs e)
        {
            AdminSystem.Prestadores.Prestador A = new AdminSystem.Prestadores.Prestador();
            A.ShowDialog();
        }
        private void pictureBox39_Click(object sender, EventArgs e)
        {
            AdminSystem.Horarios H = new AdminSystem.Horarios();
            H.ShowDialog();
        }
        private void pictureBox40_Click(object sender, EventArgs e)
        {
            AdminSystem.Festivos festivos = new AdminSystem.Festivos();
            festivos.ShowDialog();
        }
        private void pictureBox41_Click(object sender, EventArgs e)
        {
            Proveedores f = new Proveedores();
            f.ShowDialog();
        }
        private void pictureBox42_Click(object sender, EventArgs e)
        {
            AdminSystem.UsuariosSystem A = new AdminSystem.UsuariosSystem();
            A.ShowDialog();
        }
        private void pictureBox43_Click(object sender, EventArgs e)
        {
            AdminSystem.CyT C = new AdminSystem.CyT();
            C.ShowDialog();
        }
        private void pictureBox44_Click(object sender, EventArgs e)
        {
            Cie10Admin f = new Cie10Admin();
            f.ShowDialog();
        }
        private void pictureBox45_Click(object sender, EventArgs e)
        {
            UsuariosSistema f = new UsuariosSistema();
            f.ShowDialog();
        }
        private void Opciones_Load(object sender, EventArgs e)
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

                    JObject obj3 = JObject.Parse(Rol.Opciones);

                    if (obj3["Opciones"] != null)
                    {
                        panel2.Visible = obj3["Opciones"]["Compañias"] != null ?
                                            obj3["Opciones"]["Compañias"].ToString() == "A" ? true : false : false;
                        panel3.Visible = obj3["Opciones"]["CrearEditarUsuario"] != null ?
                                           obj3["Opciones"]["CrearEditarUsuario"].ToString() == "A" ? true : false : false;
                        panel4.Visible = obj3["Opciones"]["Productos"] != null ?
                                          obj3["Opciones"]["Productos"].ToString() == "A" ? true : false : false;
                        panel5.Visible = obj3["Opciones"]["Horarios"] != null ?
                                          obj3["Opciones"]["Horarios"].ToString() == "A" ? true : false : false;
                        panel6.Visible = obj3["Opciones"]["Convenios"] != null ?
                                          obj3["Opciones"]["Convenios"].ToString() == "A" ? true : false : false;
                        panel7.Visible = obj3["Opciones"]["Preferencias"] != null ?
                                         obj3["Opciones"]["Preferencias"].ToString() == "A" ? true : false : false;
                        panel8.Visible = obj3["Opciones"]["Festivos"] != null ?
                                         obj3["Opciones"]["Festivos"].ToString() == "A" ? true : false : false;
                        panel9.Visible = obj3["Opciones"]["CIE10"] != null ?
                                        obj3["Opciones"]["CIE10"].ToString() == "A" ? true : false : false;
                        panel10.Visible = obj3["Opciones"]["ECuentas"] != null ?
                                      obj3["Opciones"]["ECuentas"].ToString() == "A" ? true : false : false;
                        panel11.Visible = obj3["Opciones"]["Compras"] != null ?
                                      obj3["Opciones"]["Compras"].ToString() == "A" ? true : false : false;
                        panel12.Visible = obj3["Opciones"]["Bodegas"] != null ?
                                      obj3["Opciones"]["Bodegas"].ToString() == "A" ? true : false : false;
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
        private void pictureBox66_Click(object sender, EventArgs e)
        {
            AdminSystem.Productos P = new AdminSystem.Productos();
            P.ShowDialog();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if (Contenedor.UsuarioLogueado == "FGAMBA")
            {
                ConfigGen C = new ConfigGen();
                C.ShowDialog();
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "Su usuario no esta autorizado para cambiar la configuracion del sistema";
                MG.ShowDialog();
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            EstadoCuenta P = new EstadoCuenta();
            P.ShowDialog();
        }
    }
}
