using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class GeneratedPController
    {
        private PacientesService pacientesService;
        private CompañiaService compañiaService;
        private OMService oMService;
        private AgendaService agendaService;

        public GeneratedPController()
        {
            pacientesService = new PacientesService();
            compañiaService = new CompañiaService();
            oMService = new OMService();
            agendaService = new AgendaService();
        }

        public CXN_PACIENTES LlamarPacienteOnlyDOC(string numid)
        {
            return pacientesService.LlamarPacienteOnlyDOC(numid).GetAwaiter().GetResult();
        }

        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return compañiaService.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }

        public List<CXN_OM> getOrdenes(string tid, string nid, int cia)
        {
            return oMService.getOrdenes(tid, nid, cia).GetAwaiter().GetResult();
        }

        public List<string> ObtenerListaPrestadores()
        {
            return compañiaService.ObtenerListaPrestadores().GetAwaiter().GetResult();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }
    }
}
