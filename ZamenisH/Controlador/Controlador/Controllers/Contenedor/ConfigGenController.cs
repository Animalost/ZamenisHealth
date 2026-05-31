using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.Contenedor
{
    public class ConfigGenController
    {
        private readonly ConfSystemService confSystemService;
        public ConfigGenController()
        {
            confSystemService = new ConfSystemService();
        }

        public Dictionary<string, string> getListado()
        {
            return confSystemService.getListado().GetAwaiter().GetResult();
        }

        public bool updateImagenSystem(CXN_IMAGEN_SYSTEM I)
        {
            return confSystemService.updateImagenSystem(I).GetAwaiter().GetResult();
        }
    }
}
