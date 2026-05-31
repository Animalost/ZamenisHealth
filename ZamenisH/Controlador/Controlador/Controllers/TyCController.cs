using Controlador.Services;

namespace Controlador.Controllers
{
    public class TyCController
    {
        private readonly LoginService _loginService;

        public TyCController()
        {
            _loginService = new LoginService();
        }

        public bool AcceptTyC(string user)
        {
            return _loginService.AcceptTyC(user).GetAwaiter().GetResult();
        }
    }
}
