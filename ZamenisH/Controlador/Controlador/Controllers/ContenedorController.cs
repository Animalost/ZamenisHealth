using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public  class ContenedorController
    {
        private readonly ConfSystemService confSystemService;
        private readonly MensajeroService maensajeroService;
        private readonly LoginService loginService;

        public ContenedorController() 
        { 
            confSystemService = new ConfSystemService();
            maensajeroService = new MensajeroService();
            loginService = new LoginService();
        }

        public CXN_MESSENGER GetMessages(string User)
        {
            return maensajeroService.GetMessages(User).GetAwaiter().GetResult();
        }

        public CXN_LOGIN GetUser(string URLAPIConexion, string User)
        {
            return loginService.GetUser(URLAPIConexion, User).GetAwaiter().GetResult();
        }
        public bool LogoutAsync()
        {
            return loginService.LogoutAsync().GetAwaiter().GetResult();
        }
        public Dictionary<string, string> getListado()
        {
            return confSystemService.getListado().GetAwaiter().GetResult();
        }
        public bool saveAvatar(byte[] _avatar, string user)
        {
            return loginService.saveAvatar(_avatar, user).GetAwaiter().GetResult();
        }
    }
}
