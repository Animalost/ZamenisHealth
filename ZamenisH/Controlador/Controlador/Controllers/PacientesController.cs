using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class PacientesController
    {
        private readonly PacientesService pacientesService;

        public PacientesController()
        {
            pacientesService = new PacientesService();
        }
        public List<CXN_PACIENTES> ObtenerListaPacientes(string Nombre, string Apellido)
        {
            return pacientesService.ObtenerListaPacientes(Nombre, Apellido).GetAwaiter().GetResult();
        }
    }
}
