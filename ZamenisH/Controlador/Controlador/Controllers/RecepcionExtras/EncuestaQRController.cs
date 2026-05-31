using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class EncuestaQRController
    {
        private ConfSystemService ConfSystemService;
        private AgendaService _agendaService;
        private EncuestasService _encuestasService;
        private PacientesService _pacientesService;

        public EncuestaQRController()
        {
            ConfSystemService = new ConfSystemService();
            _agendaService = new AgendaService();
            _encuestasService = new EncuestasService();
            _pacientesService = new PacientesService();
        }

        public Dictionary<string, string> getListado()
        {
            return ConfSystemService.getListado().GetAwaiter().GetResult();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return _agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

        public bool getCantEncuestaCU(string service, string mes, int año)
        {
            return _encuestasService.getCantEncuestaCU(service, mes, año).GetAwaiter().GetResult();
        }

        public bool getCantEncuestasPaciente(int Paciente, CXN_CONFENCUESTA C)
        {
            return _encuestasService.getCantEncuestasPaciente(Paciente, C).GetAwaiter().GetResult();
        }

        public bool Actualiza_Email(int pac, string email)
        {
            return _pacientesService.Actualiza_Email(pac, email).GetAwaiter().GetResult();
        }
    }
}
