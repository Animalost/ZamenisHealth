using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.AdminSystem;
using ZamenisHealth.Facturacion;
using ZamenisHealth.INV;

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

        private void pictureBox72_Click(object sender, EventArgs e)
        {
            if (Conexion.ConectionDictionary["Recordatorios"] == "A")
            {
                Mensajeria.MainMessage M = new Mensajeria.MainMessage();
                M.ShowDialog();
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
                CXN_ROLES R = repoRoles.getRoles(Contenedor.UsuarioLogueado);
                if (R == null)
                {
                    pictureBox62.Enabled = false;
                    pictureBox63.Enabled = false;
                    pictureBox64.Enabled = false;
                    pictureBox65.Enabled = false;
                    pictureBox67.Enabled = false;
                    pictureBox68.Enabled = false;
                    pictureBox69.Enabled = false;
                    pictureBox1.Enabled = false;

                    label58.Enabled = false;
                    label57.Enabled = false;
                    label56.Enabled = false;
                    label45.Enabled = false;
                    label67.Enabled = false;
                    label61.Enabled = false;
                    label52.Enabled = false;
                    label1.Enabled = false;
                }
                else
                {
                    pictureBox62.Enabled = (R.Rol_A_Facturacion == "A" ? true : false);
                    pictureBox63.Enabled = (R.Rol_A_Cargos == "A" ? true : false);
                    pictureBox64.Enabled = (R.Rol_A_RIPS == "A" ? true : false);
                    pictureBox65.Enabled = (R.Rol_A_Anulaciones == "A" ? true : false);
                    pictureBox67.Enabled = (R.Rol_A_Adherencia == "A" ? true : false);
                    pictureBox68.Enabled = (R.Rol_A_Formatos == "A" ? true : false);
                    pictureBox69.Enabled = (R.Rol_A_Mensajero == "A" ? true : false);
                    pictureBox4.Enabled = (R.AdminFHIR == "A" ? true : false);

                    pictureBox2.Enabled = (R.Rol_A_Autorizaciones == "A" ? true : false);
                    pictureBox1.Enabled = (R.Rol_A_Autorizaciones == "A" ? true : false);

                    pictureBox3.Enabled = (R.Rol_A_Inventario == "A" ? true : false);

                    label58.Enabled = (R.Rol_A_Facturacion == "A" ? true : false);
                    label57.Enabled = (R.Rol_A_Cargos == "A" ? true : false);
                    label56.Enabled = (R.Rol_A_RIPS == "A" ? true : false);
                    label45.Enabled = (R.Rol_A_Anulaciones == "A" ? true : false);
                    label67.Enabled = (R.Rol_A_Adherencia == "A" ? true : false);
                    label61.Enabled = (R.Rol_A_Formatos == "A" ? true : false);
                    label52.Enabled = (R.Rol_A_Mensajero == "A" ? true : false);
                    label5.Enabled = (R.AdminFHIR == "A" ? true : false);

                    label2.Enabled = (R.Rol_A_Autorizaciones == "A" ? true : false);
                    label1.Enabled = (R.Rol_A_Autorizaciones == "A" ? true : false);

                    label3.Enabled = (R.Rol_A_Inventario == "A" ? true : false);
                }

                if (repoConfSystem.getListado()["IHCE"] == "A")
                {
                    label5.Visible = true;
                    pictureBox4.Visible = true;
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
            CargarBodega P = new CargarBodega();
            P.ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            FrontFHIR.EnvioFHIR f = new FrontFHIR.EnvioFHIR();
            f.ShowDialog();
        }
    }
}
