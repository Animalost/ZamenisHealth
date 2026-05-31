using DocumentosElectronicos.Controlador;
using DocumentosElectronicos.Servicio;
using Domain;
using Domain.CXN;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ZamenisHealth.Clases
{
    public static class GenerateXMLPDF
    {
        private readonly static IHelisa repoWSClients = new MHelisa();
        private readonly static IFacElectron repoFac = new MFacElectron();
        private readonly static IConfSystem repoConfSystem = new MConfSystem();


        //NOTA CREDITO
        public static void GeneratePDFSaludNC(int Prestador, string Token)
        {
            try
            {
                ReportViewer R = new ReportViewer();
                var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (var res in resources)
                    Debug.WriteLine(res);

                R.LocalReport.DataSources.Clear();
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", GeneradorXML.rTemp));
                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludNC.rdlc";
                R.SetDisplayMode(DisplayMode.PrintLayout);
                R.ZoomMode = ZoomMode.Percent;
                R.ZoomPercent = 100;
                R.Font = new System.Drawing.Font("Arial", 7);
                R.LocalReport.EnableExternalImages = true;
                R.RefreshReport();
                R.Dock = System.Windows.Forms.DockStyle.Fill;

                byte[] bytes = R.LocalReport.Render("PDF");

                string Docelectronico = GeneradorXML.PrefElectronTemp.ToString() + GeneradorXML.NumElectronTemp.ToString();
                string GetCUFE = ActualizarDocumentos.GetCUFE(Docelectronico, Prestador, "NotasCredito");
                //Radicar PDF
                GenerarBase64PDF(bytes, Prestador, GetCUFE, Docelectronico, Token);

                FileStream fss = new FileStream("C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\NC_" + GeneradorXML.DocPrestadorTemp.ToString() + "_" + Docelectronico + ".pdf", FileMode.Create);
                fss.Write(bytes, 0, bytes.Length);
                fss.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void GeneratePDFVentasNC(int Prestador, string Token)
        {
            try
            {
                ReportViewer R = new ReportViewer();
                var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (var res in resources)
                    Debug.WriteLine(res);

                R.LocalReport.DataSources.Clear();
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", GeneradorXML.rTemp));
                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludNC.rdlc";
                R.SetDisplayMode(DisplayMode.PrintLayout);
                R.ZoomMode = ZoomMode.Percent;
                R.ZoomPercent = 100;
                R.Font = new System.Drawing.Font("Arial", 7);
                R.LocalReport.EnableExternalImages = true;
                R.RefreshReport();
                R.Dock = System.Windows.Forms.DockStyle.Fill;

                byte[] bytes = R.LocalReport.Render("PDF");

                string Docelectronico = GeneradorXML.PrefElectronTemp.ToString() + GeneradorXML.NumElectronTemp.ToString();
                string GetCUFE = ActualizarDocumentos.GetCUFE(Docelectronico, Prestador, "NotasCredito");
                //Radicar PDF
                GenerarBase64PDF(bytes, Prestador, GetCUFE, Docelectronico, Token);

                FileStream fss = new FileStream("C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\NC_" + GeneradorXML.DocPrestadorTemp.ToString() + "_" + Docelectronico + ".pdf", FileMode.Create);
                fss.Write(bytes, 0, bytes.Length);
                fss.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void GeneratePDFCajaNC(int Prestador, string Token)
        {
            try
            {
                ReportViewer R = new ReportViewer();

                var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (var res in resources)
                    Debug.WriteLine(res);

                R.LocalReport.DataSources.Clear();
                R.LocalReport.DataSources.Add(new ReportDataSource("ReciboCajaDataset", GeneradorXML.rTempCaja));
                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaCajaNC.rdlc";
                R.SetDisplayMode(DisplayMode.PrintLayout);
                R.ZoomMode = ZoomMode.Percent;
                R.ZoomPercent = 100;
                R.Font = new System.Drawing.Font("Arial", 7);
                R.LocalReport.EnableExternalImages = true;
                R.RefreshReport();
                R.Dock = System.Windows.Forms.DockStyle.Fill;

                byte[] bytes = R.LocalReport.Render("PDF"); 

                string Docelectronico = GeneradorXML.PrefElectronTemp.ToString() + GeneradorXML.NumElectronTemp.ToString();
                string GetCUFE = ActualizarDocumentos.GetCUFE(Docelectronico, Prestador, "NotasCredito");
                //Radicar PDF
                GenerarBase64PDF(bytes, Prestador, GetCUFE, Docelectronico, Token);

                FileStream fss = new FileStream("C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\NC_" + GeneradorXML.DocPrestadorTemp.ToString() + "_" + Docelectronico + ".pdf", FileMode.Create);
                fss.Write(bytes, 0, bytes.Length);
                fss.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        //REENVIAR PDF
        public static void ReenviarPDFSalud(string Factura, int Prestador, string Tipo, string Token)
        {
            try
            {
                ReportViewer R = new ReportViewer();

                List<FacturasR> FacturaAseguradoraOtras = new List<FacturasR>();
                List<RCCAJA> Caja = new List<RCCAJA>();
                List<FacturacionRpt> Ventas = new List<FacturacionRpt>();

                var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (var res in resources)
                    Debug.WriteLine(res);

                R.LocalReport.DataSources.Clear();

                if (Tipo == "Salud")
                {
                    int Doc = repoFac.GetFacZam(Prestador, Factura, "Salud");
                    FacturaAseguradoraOtras = ExportarPDF.ExportarFacturaAseguradoras(Doc, Prestador, "OP");

                    R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", FacturaAseguradoraOtras));

                    if (repoConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                    {
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludIMPUESTOS.rdlc";
                    }
                    else
                    {
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc";
                    }                    
                }
                else if (Tipo == "Caja")
                {
                    int Doc = repoFac.GetFacZam(Prestador, Factura, "Caja");
                    Caja = ExportarPDF.ExportarReciboCaja(Doc);

                    R.LocalReport.DataSources.Add(new ReportDataSource("ReciboCajaDataset", Caja));
                    R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_RcCajaImpTermica.rdlc";
                }
                else if (Tipo == "Ventas")
                {
                    int Doc = repoFac.GetFacZam(Prestador, Factura, "Ventas");
                    Ventas = ExportarPDF.ExportarFacturaVentas(Doc, Prestador, "OP"); 

                    R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", Ventas));

                    if (repoConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                    {
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludIMPUESTOS.rdlc";
                    }
                    else
                    {
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc";
                    }                   
                }
                else if (Tipo == "SaludNC")
                {
                    List<FacturasR> GenerarDocumentoGraficoNC = ExportarPDF.ExportarFacturaAseguradorasNC(Factura, Prestador); 

                    R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", GenerarDocumentoGraficoNC));
                    R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludNC.rdlc";
                }
                else if (Tipo == "VentasNC")
                {
                    int Doc = repoFac.GetFacZam(Prestador, Factura, "NC");
                    List<FacturacionRpt> Exportar = ExportarPDF.ExportarFacturaVentasNC(Doc, Prestador, "OP", Factura);

                    R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", Exportar));
                    R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludNC.rdlc";
                }
                else if (Tipo == "CajaNC")
                {
                    int Doc = repoFac.GetFacZam(Prestador, Factura, "NC");
                    List<RCCAJA> Exportar = ExportarPDF.ExportarReciboCajaNC(Doc);

                    R.LocalReport.DataSources.Add(new ReportDataSource("ReciboCajaDataset", Exportar));
                    R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaCajaNC.rdlc";
                }
                else
                {
                    return;
                }
                                        
                string GetCUFE = "";

                if (Tipo == "Salud")
                {
                    GetCUFE = ActualizarDocumentos.GetCUFE(Factura, Prestador, "AseguraOtras");
                }
                else if (Tipo == "Caja")
                {
                    GetCUFE = ActualizarDocumentos.GetCUFE(Factura, Prestador, "Caja");
                }
                else if (Tipo == "Ventas")
                {
                    GetCUFE = ActualizarDocumentos.GetCUFE(Factura, Prestador, "Ventas");
                }
                else if (Tipo == "SaludNC" || Tipo == "VentasNC" || Tipo == "CajaNC")
                {
                    GetCUFE = ActualizarDocumentos.GetCUFE(Factura, Prestador, "NotasCredito");
                }
                else
                {
                    return;
                }

                R.SetDisplayMode(DisplayMode.PrintLayout);
                R.ZoomMode = ZoomMode.Percent;
                R.ZoomPercent = 100;
                R.Font = new System.Drawing.Font("Arial", 7);
                R.LocalReport.EnableExternalImages = true;
                R.RefreshReport();
                R.Dock = System.Windows.Forms.DockStyle.Fill;

                byte[] bytes = R.LocalReport.Render("PDF");

                GenerarBase64PDF(bytes, Prestador, GetCUFE, Factura, Token); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        // Para todos pasar el pdf a base 64
        public static void GenerarBase64PDF(byte[] bytes, int Prestador, string CUFE, string DocElectronico, string Token)
        {
            try
            {
                string Conversion = Convert.ToBase64String(bytes);
                RadicarPDF radicarPDF = new RadicarPDF();

                Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1PDF", Prestador);
                if (dataWS == null)
                {
                    TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "No se logro radicar el PDF, llave no encontrada en el diccionario", Formulario = "ENVIAR PDF JSON", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                    return;
                }

                string user = dataWS["User"];
                string pass = dataWS["Pass"];

                radicarPDF.SendPDF(Token, Program.URLApiConexion, user, pass, Prestador, Conversion, CUFE, DocElectronico).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "ENVIAR PDF JSON", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
