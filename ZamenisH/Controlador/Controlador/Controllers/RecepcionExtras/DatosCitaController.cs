using Controlador.Services;
using Domain.CXN;

namespace Controlador.Controllers.RecepcionExtras
{
    public class DatosCitaController
    {
        private AgendaService agendaService;
        private PacientesService pacientesService;
        private OMService oMService;

        public DatosCitaController()
        {
            agendaService = new AgendaService();
            pacientesService = new PacientesService();
            oMService = new OMService();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

        public CXN_PACIENTES LlamarPacientebyId(int pacid)
        {
            return pacientesService.LlamarPacientebyId(pacid).GetAwaiter().GetResult();
        }

        public string Calcular3(int Paciente, string TipoServicio, string Recepcion)
        {
            return agendaService.Calcular3(Paciente, TipoServicio, Recepcion).GetAwaiter().GetResult();
        }

        public bool OPendUpdate(CXN_OPEND O)
        {
            return oMService.OPendUpdate(O).GetAwaiter().GetResult();
        }

        public bool addValidacionPin(int admision, string Pin)
        {
            return agendaService.addValidacionPin(admision, Pin).GetAwaiter().GetResult();
        }

        public bool addRegAtn(int admision, string registro)
        {
            return agendaService.addRegAtn(admision, registro).GetAwaiter().GetResult();
        }
    }
}
