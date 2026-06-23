using Domain;
using Domain.CXN;
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
        private static readonly ILogin repositorioLogin = new MLogin();
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
                CXN_ROLES R = repoRoles.getRoles(Contenedor.UsuarioLogueado);
                if (R == null)
                {
                    pictureBox38.Enabled = false;
                    pictureBox39.Enabled = false;
                    pictureBox40.Enabled = false;
                    pictureBox41.Enabled = false;
                    pictureBox42.Enabled = false;
                    pictureBox43.Enabled = false;
                    pictureBox44.Enabled = false;
                    pictureBox45.Enabled = false;
                    pictureBox66.Enabled = false;

                    label15.Enabled = false;
                    label16.Enabled = false;
                    label17.Enabled = false;
                    label18.Enabled = false;
                    label19.Enabled = false;
                    label20.Enabled = false;
                    label21.Enabled = false;
                    label22.Enabled = false;
                    label54.Enabled = false;
                }
                else
                {
                    pictureBox38.Enabled = (R.Rol_O_Compañias == "A" ? true : false);
                    pictureBox39.Enabled = (R.Rol_O_Horarios == "A" ? true : false);
                    pictureBox40.Enabled = (R.Rol_O_Festivos == "A" ? true : false);
                    pictureBox41.Enabled = (R.Rol_O_Proveedores == "A" ? true : false);
                    pictureBox42.Enabled = (R.Rol_O_UsuariosSystem == "A" ? true : false);
                    pictureBox43.Enabled = (R.Rol_O_Convenios == "A" ? true : false);
                    pictureBox44.Enabled = (R.Rol_O_CIE10 == "A" ? true : false);
                    pictureBox45.Enabled = (R.Rol_O_Bodegas == "A" ? true : false);
                    pictureBox66.Enabled = (R.Rol_A_Productos == "A" ? true : false);

                    label15.Enabled = (R.Rol_O_Bodegas == "A" ? true : false);
                    label16.Enabled = (R.Rol_O_CIE10 == "A" ? true : false);
                    label17.Enabled = (R.Rol_O_Convenios == "A" ? true : false);
                    label18.Enabled = (R.Rol_O_UsuariosSystem == "A" ? true : false);
                    label19.Enabled = (R.Rol_O_Proveedores == "A" ? true : false);
                    label20.Enabled = (R.Rol_O_Festivos == "A" ? true : false);
                    label21.Enabled = (R.Rol_O_Horarios == "A" ? true : false);
                    label22.Enabled = (R.Rol_O_Compañias == "A" ? true : false);
                    label54.Enabled = (R.Rol_A_Productos == "A" ? true : false);
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
