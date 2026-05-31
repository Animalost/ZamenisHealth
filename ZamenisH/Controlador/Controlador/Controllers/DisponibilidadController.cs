using Controlador.Services;

namespace Controlador.Controllers
{
    public class DisponibilidadController
    {
        private readonly DisponibilidadService disponibilidadService;

        public DisponibilidadController(DisponibilidadService disponibilidadService)
        {
            this.disponibilidadService = disponibilidadService;
        }

        
    }
}
