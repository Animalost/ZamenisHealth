using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.FacElectron.EmbededFac;
using ZamenisHealth.Facturacion;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Administracion2 : Form
    {
        private static readonly IRoles repoRoles = new MRoles();

        public Administracion2()
        {
            InitializeComponent();
        }

        private void pictureBox55_Click(object sender, EventArgs e)
        {
            Facturar F = new Facturacion.Facturar();
            F.ShowDialog();
        }

        private void pictureBox56_Click(object sender, EventArgs e)
        {
            ReportesCopias F = new Facturacion.ReportesCopias();
            F.ShowDialog();
        }

        private void pictureBox58_Click(object sender, EventArgs e)
        {
            AdminSystem.Homologos H = new AdminSystem.Homologos();
            H.ShowDialog();
        }

        private void pictureBox59_Click(object sender, EventArgs e)
        {
            Facturacion.Facturar4Detail facturar4Detail = new Facturacion.Facturar4Detail();
            facturar4Detail.ShowDialog();
        }

        private void pictureBox60_Click(object sender, EventArgs e)
        {
            AdminSystem.GestionP gestionP = new AdminSystem.GestionP();
            gestionP.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Facturacion.FacturaAbierta0 facturaAbierta = new FacturaAbierta0();
            facturaAbierta.ShowDialog();
        }

        private void Administracion2_Load(object sender, EventArgs e)
        {
            try
            {
                CXN_ROLES R = repoRoles.getRoles(Contenedor.UsuarioLogueado);
                if (R == null)
                {
                    pictureBox55.Enabled = false;
                    pictureBox1.Enabled = false;
                    pictureBox2.Enabled = false;
                    pictureBox56.Enabled = false;
                    pictureBox58.Enabled = false;
                    pictureBox59.Enabled = false;
                    pictureBox60.Enabled = false;
                    pictureBox3.Enabled = false;

                    label64.Enabled = false;
                    label1.Enabled = false;
                    label2.Enabled = false;
                    label63.Enabled = false;
                    label60.Enabled = false;
                    label5.Enabled = false;
                    label10.Enabled = false;
                    label3.Enabled = false;
                }
                else
                {
                    pictureBox55.Enabled = (R.AdminFactura == "A" ? true : false);
                    pictureBox1.Enabled = (R.AdminFacturaAbierta == "A" ? true : false);
                    pictureBox2.Enabled = (R.AdminElectronica == "A" ? true : false);
                    pictureBox56.Enabled = (R.AdminReportes == "A" ? true : false);
                    pictureBox58.Enabled = (R.AdminHomologos == "A" ? true : false);
                    pictureBox59.Enabled = (R.AdminGPacientes == "A" ? true : false);
                    pictureBox60.Enabled = (R.AdminGrupal == "A" ? true : false);
                    pictureBox3.Enabled = (R.AdminReportesPagos == "A" ? true : false);

                    label64.Enabled = (R.AdminFactura == "A" ? true : false);
                    label1.Enabled = (R.AdminFacturaAbierta == "A" ? true : false);
                    label2.Enabled = (R.AdminElectronica == "A" ? true : false);
                    label63.Enabled = (R.AdminReportes == "A" ? true : false);
                    label60.Enabled = (R.AdminHomologos == "A" ? true : false);
                    label5.Enabled = (R.AdminGPacientes == "A" ? true : false);
                    label10.Enabled = (R.AdminGrupal == "A" ? true : false);
                    label3.Enabled = (R.AdminReportesPagos == "A" ? true : false);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }          
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            /*FacElectron.MainElectron mainElectron = new FacElectron.MainElectron();
            mainElectron.ShowDialog();*/

            FacPrincipal generarFacturaXML = new FacPrincipal();
            generarFacturaXML.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ReportesPagos reportesPagos = new ReportesPagos();
            reportesPagos.ShowDialog();
        }
    }
}
