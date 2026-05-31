using Controlador.Services;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class NoticiasController
    {
        private ConfSystemService confSystemService;

        public NoticiasController()
        {
            confSystemService = new ConfSystemService();
        }

        public Dictionary<string, string> getListado()
        {
            return confSystemService.getListado().GetAwaiter().GetResult();
        }
    }
}
