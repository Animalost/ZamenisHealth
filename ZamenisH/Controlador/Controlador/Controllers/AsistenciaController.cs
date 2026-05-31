using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class AsistenciaController
    {
        private AsistenciaService asistenciaService;

        public AsistenciaController() 
        { 
            asistenciaService = new AsistenciaService();
        }

        public List<CXN_HORARIO> Asistencia(string idNum) //HECHO
        {
            return asistenciaService.Asistencia(idNum).GetAwaiter().GetResult();
        }

        public List<CXN_HORARIO> AsistenciaLastAut(string idNum) //HECHO
        {
            return asistenciaService.AsistenciaLastAut(idNum).GetAwaiter().GetResult();
        }
    }
}
