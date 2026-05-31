using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class CrearClienteController
    {
        private AgendarCitaService agendaService;
        private PacientesService pacientesService;

        public CrearClienteController()
        {
            agendaService = new AgendarCitaService();
            pacientesService = new PacientesService();
        }

        public List<string> ListaDocs()
        {
            return agendaService.ListaDocs().GetAwaiter().GetResult();
        }

        public bool CrearClientes(CXN_PACIENTES P)
        {
            return pacientesService.CrearClientes(P).GetAwaiter().GetResult();
        }
    }
}
