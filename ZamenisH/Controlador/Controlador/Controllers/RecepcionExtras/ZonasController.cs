using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class ZonasController
    {
        private readonly ZonasService zonasService;

        public ZonasController()
        {
            zonasService = new ZonasService();
        }

        public string DepartamentoCodigo(string NomDep)
        {
            return zonasService.DepartamentoCodigo(NomDep).GetAwaiter().GetResult();
        }

        public List<CXN_ZONAS> _listadoCodigos(string code, string mun, string dep)
        {
            return zonasService._listadoCodigos(code, mun, dep).GetAwaiter().GetResult();
        }
    }
}
