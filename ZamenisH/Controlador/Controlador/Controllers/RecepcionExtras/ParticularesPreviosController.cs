using Controlador.Services;
using Domain;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class ParticularesPreviosController
    {
        private AgendarCitaService agendarCitaService;
        private CargosService cargosService;
        private ReportesService reportesService;

        public ParticularesPreviosController()
        {
            agendarCitaService = new AgendarCitaService();
            cargosService = new CargosService();
            reportesService = new ReportesService();
        }

        public List<string> ListaDocs()
        {
            return agendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }

        public List<CXN_CARGOS> cotizacionesPrevias(string tid, string id)
        {
            return cargosService.cotizacionesPrevias(tid, id).GetAwaiter().GetResult();
        }

        public List<CotizacionR> GenerarDocumento(int doc, int cia)
        {
            return reportesService.GenerarDocumento(doc, cia).GetAwaiter().GetResult();
        }
    }
}
