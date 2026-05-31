using Domain;
using Domain.CXN;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class ReportesService
    {
        public async Task<List<FacturacionRpt>> Exp_Fac_Ven(int docu_Ven, int cia, string tipo) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Tipo = tipo,
                    Docu_Ven = docu_Ven,
                    Cia = cia
                };

                return APIController.SendMessageToAPI<List<FacturacionRpt>>(datos, "/api/Reportes/Exp_Fac_Ven", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<RCCAJA>> ReciboRpt(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision
                };

                return APIController.SendMessageToAPI<List<RCCAJA>>(datos, "/api/Reportes/ReciboRpt", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<ReportesRecepcion>> Rpt_FacturasVenta(DateTime desde,
                                                                     DateTime hasta,
                                                                     int cia,
                                                                     string prestador,
                                                                     string tipo) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta),
                    Cia = cia,
                    Prestador = prestador,
                    Tipo = tipo
                };

                return APIController.SendMessageToAPI<List<ReportesRecepcion>>(datos, "/api/Reportes/Rpt_FacturasVenta", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<ReportesRecepcion>> Rpt_RecibosdeCaja(DateTime desde,
                                                                     DateTime hasta,
                                                                     int cia,
                                                                     string prestador)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta),
                    Cia = cia,
                    Prestador = prestador,
                    Tipo = ""
                };

                return APIController.SendMessageToAPI<List<ReportesRecepcion>>(datos, "/api/Reportes/Rpt_RecibosdeCaja", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<CotizacionR>> GenerarDocumento(int doc, int cia)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Cia = cia,
                    Doc = doc,
                };

                return APIController.SendMessageToAPI<List<CotizacionR>>(datos, "/api/Particulares/GenerarDocumento", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> listaPrecios(int convenio)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CompanyCode = convenio
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Ventas/listaPrecios", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<MReportes.PrntAgendas>> Genera_CitasXPac_Report(string doc, DateTime desde, DateTime hasta)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    DOC = doc,
                    Desde = desde,
                    Hasta = hasta
                };

                return APIController.SendMessageToAPI<List<MReportes.PrntAgendas>>(datos, "/api/Reportes/Genera_CitasXPac_Report", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<MReportes.PrntAgendas>> Genera_Export_Citas(string tipo_reporte_citas,
                                                                           DateTime fechaAgenda,
                                                                           string prof_med_citas)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    TReporte = tipo_reporte_citas,
                    FechaAgenda = fechaAgenda,
                    CodesProf = prof_med_citas
                };

                return APIController.SendMessageToAPI<List<MReportes.PrntAgendas>>(datos, "/api/Reportes/Genera_Export_Citas", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<FacturasR>> Fac_Export(int numero_Fac, int cia, string tipo)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    NumeroFac = numero_Fac,
                    Cia = cia,
                    Tipo = tipo
                };

                return APIController.SendMessageToAPI<List<FacturasR>>(datos, "/api/Facturacion/Fac_Export", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<FacturasR>> Exporta_DOCE_IND_Orden(int numero_Fac, int cia, string tipo)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    NumeroFac = numero_Fac,
                    Cia = cia,
                    Tipo = tipo
                };

                return APIController.SendMessageToAPI<List<FacturasR>>(datos, "/api/Facturacion/Exporta_DOCE_IND_Orden", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> RptCancelaciones(DateTime desde, DateTime hasta)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta)
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Gerencia/RptCancelaciones", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> Rpt_Ordenes(DateTime desde, DateTime hasta)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta)
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Gerencia/Rpt_Ordenes", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> Rpt_Atenciones2(DateTime desde, DateTime hasta)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta)
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Gerencia/Rpt_Atenciones2", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<GerencialR>> Rpt_Atenciones(DateTime desde, DateTime hasta)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta)
                };

                return APIController.SendMessageToAPI<List<GerencialR>>(datos, "/api/Gerencia/Rpt_Atenciones", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> Rpt_Ing_Ger(DateTime desde, DateTime hasta)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta)
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Gerencia/Rpt_Ing_Ger", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> Rpt_Car_Tot(DateTime desde, DateTime hasta)
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta)
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Gerencia/Rpt_Car_Tot", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
