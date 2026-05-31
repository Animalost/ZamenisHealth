using APIController.Services.Images;
using APIController.Services.Login;
using Domain.CXN;
using System.Collections.Generic;

namespace APIController.Images
{
    public class ImagesController
    {
        private ImagesService ImagesService;
        private LoginService loginService;

        public ImagesController()
        {
            ImagesService = new ImagesService();
            loginService = new LoginService();
        }

        public int insertImageAPI(CXN_IMAGENES I, string cadena)
        {
            return ImagesService.insertImageAPI(I, cadena).GetAwaiter().GetResult();
        }
        public bool Loguear(string URLAPIConexion, string User, string Cadena)
        {
            return loginService.Loguear(URLAPIConexion, User, Cadena).GetAwaiter().GetResult();
        }
        public List<CXN_IMAGENES> GetImages(int pacid, string cadena)
        {
            return ImagesService.GetImages(pacid, cadena).GetAwaiter().GetResult();
        }
        public string ViewImage(string ruta)
        {
            return ImagesService.ViewImage(ruta).GetAwaiter().GetResult();
        }
        public List<CXN_IMAGENES> getImagesByAdmition(int admision, string cadena)
        {
            return ImagesService.getImagesByAdmition(admision, cadena).GetAwaiter().GetResult();
        }
    }
}
