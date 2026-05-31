using Controlador.Services;
using Domain.CXN;

namespace Controlador.Controllers.Contenedor
{
    public class MisDatosController
    {
        private readonly LoginService _loginService;

        public MisDatosController()
        {
            _loginService = new LoginService();
        }

        public CXN_LOGIN GetUser(string URLAPIConexion, string User)
        {
            return _loginService.GetUser(URLAPIConexion, User).GetAwaiter().GetResult();
        }

        public bool saveAvatar(byte[] _avatar, string user)
        {
            return _loginService.saveAvatar(_avatar, user).GetAwaiter().GetResult();
        }

        public int ActualizarMisDatos(CXN_LOGIN L)
        {
            return _loginService.ActualizarMisDatos(L).GetAwaiter().GetResult();
        }
    }
}
