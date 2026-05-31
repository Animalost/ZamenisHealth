using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class CAsistenciaController
    {
        private AsistenciaService asistenciaService;

        public CAsistenciaController()
        {
            asistenciaService = new AsistenciaService();
        }

        public List<CXN_CIA> certificadoAsistencia(int admision, string texto)
        {
            return asistenciaService.certificadoAsistencia(admision, texto).GetAwaiter().GetResult();
        }
    }
}
