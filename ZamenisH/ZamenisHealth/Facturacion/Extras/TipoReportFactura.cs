using DocumentosElectronicos.Servicio;
using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion.Extras
{
    public partial class TipoReportFactura : Forma2
    {
        private static readonly IGenerales repoGen = new MGenerales();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();

        private bool EsVenta;
        private List<FacturasR> AseguradorasYParticulares = null;
        private List<FacturacionRpt> Exportar = null;

        private int Cia = 0;
        private int Fac = 0;
        private MensajesGeneral MG;

        public TipoReportFactura(List<FacturacionRpt> V, List<FacturasR> F, bool _Ventas)
        {
            InitializeComponent();
            EsVenta = _Ventas;
            AseguradorasYParticulares = F;
            Exportar = V;
        }
        public TipoReportFactura(int fac, int cia)
        {
            InitializeComponent();
            EsVenta = false;
            AseguradorasYParticulares = null;
            Exportar = null;

            Cia = cia;
            Fac = fac;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //OTRAS
                if (Fac != 0 && Cia != 0)
                {
                    DialogResult result = MessageBox.Show("Marque SI para mostrar el documento con todos los impuestos o " +
                        "Marque NO para solo incluir el IVA", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_ExportOtrosServiciosTODOIMPUESTOS(Fac, Cia, "OP", true);
                        if (GenerarDocumentoGrafico != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                          "ZamenisHealth.Reportes.RDLC_OtrasFacturas.rdlc",
                                                          GenerarDocumentoGrafico);
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 0;
                            MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                            MG.ShowDialog();
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_ExportOtrosServiciosTODOIMPUESTOS(Fac, Cia, "OP", false);
                        if (GenerarDocumentoGrafico != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                          "ZamenisHealth.Reportes.RDLC_OtrasFacturasSOLOIVA.rdlc",
                                                          GenerarDocumentoGrafico);
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 0;
                            MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccion Invalida";
                        MG.ShowDialog();
                    }
                }
                else
                {
                    //ASEGURADORAS Y PARTICULARES GRAFICA ZAMENIS OP
                    if (EsVenta == false && Exportar == null && AseguradorasYParticulares != null)
                    {
                        DialogResult result = MessageBox.Show("Marque SI para mostrar el documento con todos los impuestos o " +
                      "Marque NO para no mostrar impuestos", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                      "ZamenisHealth.Reportes.RDLC_FacturaIMPUESTOS.rdlc",
                                                      AseguradorasYParticulares);
                        }
                        if (result == DialogResult.No)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                      "ZamenisHealth.Reportes.RDLC_Factura.rdlc",
                                                      AseguradorasYParticulares);
                        }                       
                    }
                    //VENTAS
                    else if (EsVenta == true && Exportar != null && AseguradorasYParticulares == null)
                    {
                        Thread thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro exportar el documento, ingrese por la opcion de copias para generarlo";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                        return;
                    }
                }               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void M()
        {
            try
            {
                DialogResult result = MessageBox.Show("Marque SI para mostrar el documento con todos los impuestos o " +
                      "Marque NO para no mostrar impuestos", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ConfigForm.GenerarReportViewer("Dataset_Facturacion",
                                               "ZamenisHealth.Reportes.RDLC_FacRecepcionIMPUESTOS.rdlc",
                                               Exportar);
                }
                if (result == DialogResult.No)
                {
                    ConfigForm.GenerarReportViewer("Dataset_Facturacion",
                                               "ZamenisHealth.Reportes.RDLC_FacRecepcion.rdlc",
                                               Exportar);
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //ASEGURADORAS Y PARTICULARES GRAFICA ZAMENIS OP
                if (EsVenta == false && Exportar == null && AseguradorasYParticulares != null)
                {
                    if (repoConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                    {
                        ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                   "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludIMPUESTOS.rdlc",
                                                   AseguradorasYParticulares);
                    }
                    else
                    {
                        ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                   "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc",
                                                   AseguradorasYParticulares);
                    }                    
                }
                //VENTAS
                else if (EsVenta == true && Exportar != null && AseguradorasYParticulares == null)
                {
                    List<FacturasR> rTemp = new List<FacturasR>();

                    string QrElectron = "NumFac:" + Exportar[0].NumFac + "\r\n" +
                          "FecFac:" + Convert.ToDateTime(Exportar[0].FechaBase).ToString("yyyy-MM-dd") + "\r\n" +
                          "HorFac:" + Convert.ToDateTime(Exportar[0].Hora).ToString("hh:mm:ss tt") + "\r\n" +
                          "NitFac:" + Exportar[0].EmpresaIdentificacion.ToString() + "\r\n" +
                          "DocAdq:" + Exportar[0].PacienteAseguradora + "\r\n" +
                          "ValFac:" + Convert.ToInt32(Exportar[0].VrNetoaPagar) + "\r\n" + //total antes de iva
                          "ValIva" + "0" + "\r\n" + //Total IVA 
                          "ValOtroIm:" + "0" + "\r\n" +
                          "ValTolFac" + Convert.ToInt32(Exportar[0].VrNetoaPagar) + "\r\n" +
                          "CUFE:" + Exportar[0].CUFE + "\r\n" +
                          "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Exportar[0].CUFE;


                    Image Code_QR_Fac_CUFE = repoGen.CodifyQR(QrElectron);

                    foreach (var item in Exportar)
                    {
                        rTemp.Add(new FacturasR
                        {
                            Letras = item.ValorLetras,
                            Car_Cod = item.CodigoProd,
                            Car_Item = item.ItemProd,
                            Cantidad = Convert.ToInt32(item.CantidadProd),
                            Car_Val_Un = Convert.ToInt32(item.VrUnitarioProd),
                            Total = Convert.ToInt32(item.VrTotalProd),
                            EmpresaDireccion = item.EmpresaDireccion,
                            EmpresaTelefono = item.EmpresaTelefono,
                            EmpresaIdentificacion = item.EmpresaIdentificacion,
                            Fac_Num_Fac = 0,
                            PacienteNombre = item.PacienteNombre,
                            PacienteDireccion = item.PacienteDireccion,
                            PacienteIdentificacion = item.PacienteIdentificacion,
                            PacienteTelefono = item.PacienteTelefono,
                            PacienteAseguradora = item.PacienteNombre,
                            Ase_NitCia = item.PacienteIdentificacion,
                            Ase_DVNitCia = "",
                            FechaBase = item.FechaBase,
                            Fac_Fecha_Des = Convert.ToDateTime(item.FechaBase),
                            Fac_Fecha_Has = Convert.ToDateTime(item.FechaBase),
                            Fac_Num_Aut = "",
                            Ase_Telefono = item.PacienteTelefono,
                            Ase_Direccion = item.PacienteDireccion,
                            Fac_Res = item.Resolucion,
                            Fac_Observa = "",
                            Fac_Descuento = item.Dcto,
                            Fac_Total = Convert.ToInt32(item.VrNetoaPagar),
                            Fac_Neto = Convert.ToInt32(item.VrNetoaPagar),
                            Usuario = item.UsuarioFactura,
                            Com_Logo = item.Logo,
                            Cufe = item.CUFE,
                            QRCufe = repoGen.GetBytes(Code_QR_Fac_CUFE),
                            EmpresaNombre = item.NumFac.ToString(),

                            Com_Direccion = item.Com_Direccion,
                            DocE_1 = item.PacienteAseguradora,
                            DocE_2 = item.ProfesionalNombre,
                            DocE_3 = item.ProfesionalNombre,
                            DocE_4 = item.Com_Resolucion_Electron,
                            ProfesionalNombre = item.PrefijoElectron,
                            DocE_5 = Convert.ToInt32(item.NumElectron),

                            NombrePrestador = item.EmpresaNombre
                        });
                    }

                    ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                   "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc",
                                                   rTemp);
                }
                //OTRAS
                else if (EsVenta == false && Exportar == null && AseguradorasYParticulares == null)
                {
                    List<FacturasR> R = ExportarPDF.ExportarFacturaOtras(Fac, Cia, "OP", false);

                    if (R == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "";
                        MG.ShowDialog();
                        return;
                    }

                    ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                  "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaOtras.rdlc",
                                                  R);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro exportar el documento, ingrese por la opcion de copias para generarlo";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                    return;
                }           
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void TipoReportFactura_Load(object sender, EventArgs e)
        {
            try
            {
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                Titulo.Text = "Tipo de Reporte";
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
