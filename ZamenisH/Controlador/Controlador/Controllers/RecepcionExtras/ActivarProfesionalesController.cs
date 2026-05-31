using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class ActivarProfesionalesController
    {
        private BodegaService bodegaService;

        public ActivarProfesionalesController()
        {
            bodegaService = new BodegaService();
        }

        public List<CXN_BODEGAS> Filtrar()
        {
            return bodegaService.Filtrar().GetAwaiter().GetResult();
        }

        public bool ActivarDesactivarBodega(int Bode, string Estado)
        {
            return bodegaService.ActivarDesactivarBodega(Bode, Estado).GetAwaiter().GetResult();    
        }
    }
}
