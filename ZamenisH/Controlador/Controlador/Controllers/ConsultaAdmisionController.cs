using Controlador.Services;
using Domain.CXN;

namespace Controlador.Controllers
{
    public class ConsultaAdmisionController
    {
        private AgendaService agendaService;

        public ConsultaAdmisionController()
        {
            agendaService = new AgendaService();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro) //HECHO
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

    }
}
