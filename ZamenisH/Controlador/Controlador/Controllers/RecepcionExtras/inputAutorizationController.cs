using Controlador.Services;

namespace Controlador.Controllers.RecepcionExtras
{
    public class inputAutorizationController
    {
        private AgendaService agendaService;

        public inputAutorizationController()
        {
            agendaService = new AgendaService();
        }

        public bool addAutroizacion(int horid, string autroizacion, int Cantidad)
        {
            return agendaService.addAutroizacion(horid, autroizacion, Cantidad).GetAwaiter().GetResult();
        }
    }
}
