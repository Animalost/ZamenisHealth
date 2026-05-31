using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class CancelacionesWEBController
    {
        private AgendaService agendaService;
        private PacientesService pacientesService;

        public CancelacionesWEBController()
        {
            agendaService = new AgendaService();
            pacientesService = new PacientesService();
        }

        public List<CXN_HORARIO> consultaCancelaWEB(string Document)
        {
            return agendaService.consultaCancelaWEB(Document).GetAwaiter().GetResult();
        }

        public CXN_PACIENTES LlamarPacienteOnlyDOC(string numid)
        {
            return pacientesService.LlamarPacienteOnlyDOC(numid).GetAwaiter().GetResult();
        }
    }
}
