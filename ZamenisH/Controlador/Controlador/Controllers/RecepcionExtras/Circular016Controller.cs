using Controlador.Services;
using System;

namespace Controlador.Controllers.RecepcionExtras
{
    public class Circular016Controller
    {
        private AgendaService agendaService;

        public Circular016Controller()
        {
            agendaService = new AgendaService();
        }

        public bool insert016(int pacid, DateTime fecha)
        {
            return agendaService.insert016(pacid, fecha).GetAwaiter().GetResult();
        }
    }
}
