using Domain;
using Newtonsoft.Json.Linq;
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
            ReportesCopias F = new Facturacion.ReportesCopias(true);
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
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Facturacion.FacturaAbierta0 facturaAbierta = new FacturaAbierta0();
            facturaAbierta.ShowDialog();
        }
        private void Administracion2_Load(object sender, EventArgs e)
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

                    if (obj2["Administracion"]["AdministracionDetalles"] != null)
                    {
                        panel2.Visible = obj2["Administracion"]["AdministracionDetalles"]["GenerarFactura"] != null ?
                                        obj2["Administracion"]["AdministracionDetalles"]["GenerarFactura"].ToString() == "A" ? true : false : false;
                        panel3.Visible = obj2["Administracion"]["AdministracionDetalles"]["Reportes"] != null ?
                                        obj2["Administracion"]["AdministracionDetalles"]["Reportes"].ToString() == "A" ? true : false : false;
                        panel4.Visible = obj2["Administracion"]["AdministracionDetalles"]["DetalleGrupal"] != null ?
                                        obj2["Administracion"]["AdministracionDetalles"]["DetalleGrupal"].ToString() == "A" ? true : false : false;
                        panel5.Visible = obj2["Administracion"]["AdministracionDetalles"]["FacturaAbierta"] != null ?
                                        obj2["Administracion"]["AdministracionDetalles"]["FacturaAbierta"].ToString() == "A" ? true : false : false;
                        panel6.Visible = obj2["Administracion"]["AdministracionDetalles"]["Homologos"] != null ?
                                        obj2["Administracion"]["AdministracionDetalles"]["Homologos"].ToString() == "A" ? true : false : false;
                        panel8.Visible = obj2["Administracion"]["AdministracionDetalles"]["FacturacionElectronica"] != null ?
                                        obj2["Administracion"]["AdministracionDetalles"]["FacturacionElectronica"].ToString() == "A" ? true : false : false;
                        panel7.Visible = obj2["Administracion"]["AdministracionDetalles"]["ReportesPagos"] != null ?
                                        obj2["Administracion"]["AdministracionDetalles"]["ReportesPagos"].ToString() == "A" ? true : false : false;
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
