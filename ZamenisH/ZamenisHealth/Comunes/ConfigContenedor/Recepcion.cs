using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Recepcion : Form
    {
        private static readonly IRoles repoRoles = new MRoles();
        private ZamenisHealth.Recepcion.Agenda A;

        public bool aa;
        public Recepcion()
        {
            InitializeComponent();

        }

        private void pictureBox47_Click(object sender, EventArgs e)
        {
            if (A != null)
            {
                A.Dispose();
                A.Close();
            }               

            A = new ZamenisHealth.Recepcion.Agenda();
            A.ShowDialog();
        }
        private void pictureBox48_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.CrearEditarPaciente A =new ZamenisHealth.Recepcion.CrearEditarPaciente();
            A.ShowDialog();
        }

        private void pictureBox49_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Asistencia A = new ZamenisHealth.Recepcion.Asistencia();
            A.ShowDialog();
        }

        private void pictureBox50_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Ventas.BuscarPaciente V = new ZamenisHealth.Recepcion.Ventas.BuscarPaciente();
            V.ShowDialog();
        }

        private void pictureBox51_Click(object sender, EventArgs e)
        {
            Medicina.Cargos C = new Medicina.Cargos(false);
            C.ShowDialog();
        }

        private void pictureBox52_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.DocumentosCopias T = new ZamenisHealth.Recepcion.DocumentosCopias();
            T.ShowDialog();
        }

        private void pictureBox53_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Extras.Precios A = new ZamenisHealth.Recepcion.Extras.Precios();
            A.ShowDialog();
        }

        private void pictureBox54_Click(object sender, EventArgs e)
        {
            Comunes.MensajeroSend M = new Comunes.MensajeroSend();
            M.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Particulares P = new ZamenisHealth.Recepcion.Particulares();
            P.ShowDialog();
        }

        private void Recepcion_Load(object sender, EventArgs e)
        {
            try
            {
                CXN_ROLES R = repoRoles.getRoles(Contenedor.UsuarioLogueado);
                if (R == null)
                {
                    pictureBox47.Enabled = false;
                    pictureBox48.Enabled = false;
                    pictureBox49.Enabled = false;
                    pictureBox50.Enabled = false;
                    pictureBox51.Enabled = false;
                    pictureBox52.Enabled = false;
                    pictureBox53.Enabled = false;
                    pictureBox1.Enabled = false;

                    label3.Enabled = false;
                    label4.Enabled = false;
                    label6.Enabled = false;
                    label7.Enabled = false;
                    label8.Enabled = false;
                    label9.Enabled = false;
                    label66.Enabled = false;
                    label1.Enabled = false;
                }
                else
                {
                    pictureBox47.Enabled = (R.Rol_R_Agenda_R == "A" ? true : false);
                    pictureBox48.Enabled = (R.Rol_R_CrearPacientes == "A" ? true : false);
                    pictureBox49.Enabled = (R.Rol_R_Asistencia == "A" ? true : false);
                    pictureBox50.Enabled = (R.Rol_R_Ventas == "A" ? true : false);
                    pictureBox51.Enabled = (R.Rol_R_Cargos == "A" ? true : false);
                    pictureBox52.Enabled = (R.Rol_R_Copias == "A" ? true : false);
                    pictureBox53.Enabled = (R.Rol_R_Precios == "A" ? true : false);
                    pictureBox1.Enabled = (R.Rol_R_Cotizaciones == "A" ? true : false);

                    label3.Enabled = (R.Rol_R_Agenda_R == "A" ? true : false);
                    label4.Enabled = (R.Rol_R_CrearPacientes == "A" ? true : false);
                    label6.Enabled = (R.Rol_R_Asistencia == "A" ? true : false);
                    label7.Enabled = (R.Rol_R_Ventas == "A" ? true : false);
                    label8.Enabled = (R.Rol_R_Cargos == "A" ? true : false);
                    label9.Enabled = (R.Rol_R_Copias == "A" ? true : false);
                    label66.Enabled = (R.Rol_R_Precios == "A" ? true : false);
                    label1.Enabled = (R.Rol_R_Cotizaciones == "A" ? true : false);
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }   
    }
}
