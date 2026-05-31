using Controlador.Services;

namespace Controlador.Controllers.RecepcionExtras
{
    public  class AgendadosDiaController
    {
        private AgendaService agendaService;

        public AgendadosDiaController()
        {
            agendaService = new AgendaService();
        }

        public bool CancelacionInterna(string razon, string user, int horId, string motivo)
        {
            return agendaService.CancelacionInterna(razon, user, horId, motivo).GetAwaiter().GetResult();    
        }
    }
}
