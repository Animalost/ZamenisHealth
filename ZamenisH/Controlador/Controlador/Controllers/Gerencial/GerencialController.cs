using Controlador.Services;
using Domain;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers.Gerencial
{
    public class GerencialController
    {
        private readonly GerenciaService _service;
        private readonly AgendarCitaService _agendarCitaService;
        private readonly CompañiaService _compa;
        private readonly FacturacionService _facturacionService;
        private readonly ReportesService reportesService;
        private readonly RcCajaService rcCajaService;
        private readonly EstadisticaService ecstadisticaService;

        public GerencialController()
        {
            _service = new GerenciaService();
            _agendarCitaService = new AgendarCitaService();
            _compa = new CompañiaService();
            _facturacionService = new FacturacionService();
            reportesService = new ReportesService();
            rcCajaService = new RcCajaService();
            ecstadisticaService = new EstadisticaService();
        }

        public string Individual(DateTime desde, DateTime hasta, string doc)
        {
            return _service.Individual(desde, hasta, doc).GetAwaiter().GetResult();
        }

        public string Total(DateTime desde, DateTime hasta)
        {
            return _service.Total(desde, hasta).GetAwaiter().GetResult();
        }

        public List<string> ListaDocs()
        {
            return _agendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }

        public List<string> ObtenerListaPrestadores()
        {
            return _compa.ObtenerListaPrestadores().GetAwaiter().GetResult();
        }

        public List<CXN_FACTURA> Filter(string tipo, string tID, string iD, int cia)
        {
            return _facturacionService.Filter(tipo, tID, iD, cia).GetAwaiter().GetResult();
        }

        public List<FacturasR> Fac_Export(int numero_Fac, int cia, string tipo)
        {
            return reportesService.Fac_Export(numero_Fac, cia, tipo).GetAwaiter().GetResult();
        }

        public List<FacturasR> Exporta_DOCE_IND_Orden(int numero_Fac, int cia, string tipo)
        {
            return reportesService.Exporta_DOCE_IND_Orden(numero_Fac, cia, tipo).GetAwaiter().GetResult();
        }

        public List<RCCAJA> ReciboRpt(int admision)
        {
            return reportesService.ReciboRpt(admision).GetAwaiter().GetResult();
        }

        public List<CXN_RC_CAJA> RCCAJAS(string tid, string id)
        {
            return rcCajaService.RCCAJAS(tid, id).GetAwaiter().GetResult();
        }

        public List<ExportInExcel> RptRecPaciente(DateTime desde, DateTime hasta, string tipoReporte)
        {
            return ecstadisticaService.RptRecPaciente(desde, hasta, tipoReporte).GetAwaiter().GetResult();
        }

        public string RptCancelaciones(DateTime desde, DateTime hasta)
        {
            return reportesService.RptCancelaciones(desde, hasta).GetAwaiter().GetResult();
        }

        public string Rpt_Ordenes(DateTime desde, DateTime hasta)
        {
            return reportesService.Rpt_Ordenes(desde, hasta).GetAwaiter().GetResult();
        }

        public string Rpt_Atenciones2(DateTime desde, DateTime hasta)
        {
            return reportesService.Rpt_Atenciones2(desde, desde).GetAwaiter().GetResult();
        }

        public List<GerencialR> Rpt_Atenciones(DateTime desde, DateTime hasta)
        {
            return reportesService.Rpt_Atenciones(desde, desde).GetAwaiter().GetResult();
        }

        public string Rpt_Ing_Ger(DateTime desde, DateTime hasta)
        {
            return reportesService.Rpt_Ing_Ger(desde, desde).GetAwaiter().GetResult();
        }

        public string Rpt_Car_Tot(DateTime desde, DateTime hasta)
        {
            return reportesService.Rpt_Car_Tot(desde, desde).GetAwaiter().GetResult();
        }

        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return _compa.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }
    }
}
