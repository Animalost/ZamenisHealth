using Controlador.Services;

namespace Controlador.Controllers
{
    public class ClavesController
    {
        private readonly LoginService _loginService;

        public ClavesController()
        {
            _loginService = new LoginService();
        }

        public bool changeClave(string User, string Pass)
        {
            return _loginService.changeClave(User, Pass).GetAwaiter().GetResult();
        }
    }
}
