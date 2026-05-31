using Domain.Licence;

namespace Controlador
{
    public class InicioController
    {
        private readonly InicioService service;

        public InicioController()
        {
            service = new InicioService();
        }

        public ZHealth GetLicence(string serial, string UrlBase)
        {
            return service.GetLicence(serial, UrlBase).GetAwaiter().GetResult();
        }
    }
}
