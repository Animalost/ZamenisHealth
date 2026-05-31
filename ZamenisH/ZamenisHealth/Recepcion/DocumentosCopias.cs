using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Facturacion;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion
{
    public partial class DocumentosCopias : Forma2
    {
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IVentas repositorioVentas = new MVentas();
        private static readonly IRcCaja repositorioRcCaja = new MRcCaja();
        private static readonly IReportes repoReportes = new MReportes();

        private MensajesGeneral MG;
        private int Cia;

        public DocumentosCopias()
        {
            InitializeComponent();
        }

        private void btnZamenis5_ButtonClick(object sender, EventArgs e)
        {
            ReportesCopias3 reportesCopias3 = new ReportesCopias3(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, comboBox2.Text);
            reportesCopias3.ShowDialog();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA Idprest = repositorioCompañias.getPrestadorbyName(comboBox3.Text);

                string Seleccion = comboBox1.Text;

                if (Idprest == null)
                {
                    MessageBox.Show("Error en compañias", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Reportes.Maestro maestro = new Reportes.Maestro();

                switch (Seleccion)
                {
                    case "Orden de Pedido":
                        comboBox1.Text = "";

                        List<FacturacionRpt> Exportar = repositorioVentas.Exp_Fac_Ven(Convert.ToInt32(textBox1.Text), Convert.ToInt32(Idprest.Com_Identificador), "OP");
                        if (Exportar == null)
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 0;
                            MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                            MG.ShowDialog();
                            return;
                        }

                        Facturacion.Extras.TipoReportFactura R = new Facturacion.Extras.TipoReportFactura(Exportar, null, true);
                        R.ShowDialog();

                        break;

                    case "Recibo de Caja":
                        List<RCCAJA> Exporta = repositorioRcCaja.ReciboRpt(Convert.ToInt32(textBox1.Text));
                        if (Exporta != null)
                        {
                            comboBox1.Text = "";

                            ConfigForm.GenerarReportViewer("ReciboCajaDataset",
                                          "ZamenisHealth.Reportes.RDLC_RcCajaImpTermica.rdlc",
                                          Exporta);
                        }
                        else
                        {
                            MessageBox.Show("Hubo un inconveniente con este recibo, ingrese por copias recepcion o consulte el administrador del sistema",
                                "No se logro exportar",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        break;                   

                    default:
                        MessageBox.Show("Debe seleccionar un tipo de reporte", "Verifique la opcion seleccionada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                CierresCaja cierresCaja = new CierresCaja(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, Cia);
                cierresCaja.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                CierresCajaPrevios cierresCaja = new CierresCajaPrevios();
                cierresCaja.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                string Seleccion = comboBox2.Text;


                CXN_CIA Cia = repositorioCompañias.getPrestadorbyName(comboBox3.Text);

                if (Cia == null)
                {
                    MessageBox.Show("Error en compañias", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                switch (Seleccion)
                {
                    case "Recibos de Caja":
                        Rpt_RecibosdeCaja(dateTimePicker1.Value.Date,
                            dateTimePicker2.Value.Date,
                            Cia.Com_Identificador,
                            comboBox3.Text);
                        break;

                    case "Ordenes de Pedido":
                        Rpt_FacturasVenta(dateTimePicker1.Value.Date,
                            dateTimePicker2.Value.Date,
                            Cia.Com_Identificador,
                            comboBox3.Text,
                            "OP");
                        break;

                    case "Facturas Particulares":
                        List<FacturacionReports> Export = repoReportes.Exportar(dateTimePicker1.Value,
                                                                     dateTimePicker2.Value,
                                                                     Cia.Com_Identificador,
                                                                     99,
                                                                     "OP");
                        if (Export == null)
                        {
                            MensajesGeneral M = new MensajesGeneral();
                            M.TipoImagen = 0;
                            M.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                            M.ShowDialog();
                            return;
                        }

                        if (comboBox4.Text == "Normal")
                        {
                            ConfigForm.GenerarReportViewer("DataSet1", "ZamenisHealth.Reportes.RDLC_ServFacturados.rdlc", Export);
                        }
                        else
                        {
                            List<FacturacionReports> ExportaRpt2 = Export.Where(x => x.PacienteTelefono == comboBox4.Text).ToList();
                            ConfigForm.GenerarReportViewer("DataSet1", "ZamenisHealth.Reportes.RDLC_ServFacturados.rdlc", ExportaRpt2);
                        }

                        
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void DocumentosCopias_Load(object sender, EventArgs e)
        {
            try
            {
                
                Titulo.Text = "Copias Documentos";

                List<CXN_CIA> prestadores = repositorioCompañias.getAllCompañias();

                if (prestadores != null)
                {
                    foreach (var i in prestadores)
                    {
                        comboBox3.Items.Add(i.Com_Nombre);
                    }

                    comboBox3.SelectedIndex = 0;
                }

                comboBox4.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_FacturasVenta(DateTime Desde, DateTime Hasta, int Cia, string Prestador, string Tipo)
        {
            try
            {
                List<ReportesRecepcion> ExportaRpt =  repositorioVentas.Rpt_FacturasVenta(Desde,
                                                                     Hasta,
                                                                     Cia,
                                                                     Prestador,
                                                                     Tipo);
                
                if (ExportaRpt == null)
                {
                    MessageBox.Show("No hay informacion para exportar", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (comboBox4.Text == "Normal")
                {
                    ConfigForm.GenerarReportViewer("DataSet_ReportGeneral",
                                                "ZamenisHealth.Reportes.RDLC_ReporteGeneralRecepcion.rdlc",
                                                ExportaRpt);
                }
                else
                {
                    List<ReportesRecepcion> ExportaRpt2 = ExportaRpt.Where(x => x.PacienteTelefono == comboBox4.Text).ToList();
                    ConfigForm.GenerarReportViewer("DataSet_ReportGeneral",
                                                "ZamenisHealth.Reportes.RDLC_ReporteGeneralRecepcion.rdlc",
                                                ExportaRpt2);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_RecibosdeCaja(DateTime Desde, DateTime Hasta, int Cia, string Prestador)
        {
            try
            {
                List<ReportesRecepcion> ExportaRpt = repositorioVentas.Rpt_RecibosdeCaja(Desde,
                                                                                         Hasta,
                                                                                         Cia,
                                                                                         Prestador,
                                                                                         (checkBox1.Checked == true ? true : false));
                
                if (ExportaRpt == null)
                {
                    MessageBox.Show("No hay informacion para exportar", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (comboBox4.Text == "Normal")
                {
                    ConfigForm.GenerarReportViewer("DataSet_ReportGeneral",
                                             "ZamenisHealth.Reportes.RDLC_ReporteGeneralRecepcion.rdlc",
                                             ExportaRpt);
                }
                else
                {
                    List<ReportesRecepcion> ExportaRpt2 = ExportaRpt.Where(x => x.PacienteTelefono == comboBox4.Text).ToList();
                    ConfigForm.GenerarReportViewer("DataSet_ReportGeneral",
                                            "ZamenisHealth.Reportes.RDLC_ReporteGeneralRecepcion.rdlc",
                                            ExportaRpt2);
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }    
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cia = repositorioCompañias.getPrestadorbyName(comboBox3.Text).Com_Identificador;
        }
    }
}
