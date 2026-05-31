using Controlador.Services;
using Domain;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class DocumentosCopiasController
    {
        private CompañiaService compañiaService;
        private ReportesService reportesService;

        public DocumentosCopiasController()
        {
            compañiaService = new CompañiaService();
            reportesService = new ReportesService();
        }

        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return compañiaService.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }

        public List<FacturacionRpt> Exp_Fac_Ven(int docu_Ven, int cia, string tipo)
        {
            return reportesService.Exp_Fac_Ven(docu_Ven, cia, tipo).GetAwaiter().GetResult();
        }

        public List<RCCAJA> ReciboRpt(int admision)
        {
            return reportesService.ReciboRpt(admision).GetAwaiter().GetResult();
        }

        public List<string> ObtenerListaPrestadores()
        {
            return compañiaService.ObtenerListaPrestadores().GetAwaiter().GetResult();
        }

        public List<ReportesRecepcion> Rpt_FacturasVenta(DateTime desde,
                                                                     DateTime hasta,
                                                                     int cia,
                                                                     string prestador,
                                                                     string tipo)
        {
            return reportesService.Rpt_FacturasVenta(desde, hasta, cia, prestador, tipo).GetAwaiter().GetResult();  
        }

        public List<ReportesRecepcion> Rpt_RecibosdeCaja(DateTime desde,
                                                                   DateTime hasta,
                                                                   int cia,
                                                                   string prestador)
        {
            return reportesService.Rpt_RecibosdeCaja(desde, hasta, cia, prestador).GetAwaiter().GetResult();
        }
    }
}
