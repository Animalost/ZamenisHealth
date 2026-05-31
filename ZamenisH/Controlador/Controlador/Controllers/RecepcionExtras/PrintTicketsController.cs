using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class PrintTicketsController
    {
        private readonly AgendaService agendaService;
        private readonly ConfSystemService confSystemService;

        public PrintTicketsController()
        {
            agendaService = new AgendaService();
            confSystemService = new ConfSystemService();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

        public Dictionary<string, string> getListado()
        {
            return confSystemService.getListado().GetAwaiter().GetResult();
        }
    }
}
