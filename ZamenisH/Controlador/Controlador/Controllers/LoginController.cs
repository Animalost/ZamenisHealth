using Controlador.Services;
using Domain.CXN;

namespace Controlador.Controllers
{
    public class LoginController
    {
        private readonly LoginService loginService;
     
        public LoginController() 
        {
            loginService = new LoginService();
        }

        public CXN_LOGIN Loguear(string URLAPIConexion, string User, string Pass, string Ciudad)
        {
            return loginService.Loguear(URLAPIConexion, User, Pass, Ciudad).GetAwaiter().GetResult();
        }
        public CXN_LOGIN GetUser(string URLAPIConexion, string User)
        {
            return loginService.GetUser(URLAPIConexion, User).GetAwaiter().GetResult();
        }
        public bool Authorization(string key, string iv)
        {
            return APIController.CreateAuthorization(key, iv, "/api/auth/Authorization").GetAwaiter().GetResult();
        }        
    }
}
