using Controlador.Services;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class MensajeroSendController
    {
        private readonly MensajeroService _service;
        private readonly LoginService loginService;

        public MensajeroSendController() 
        { 
            _service = new MensajeroService();
            loginService = new LoginService();
        }

        public CXN_LOGIN GetUser(string URLAPIConexion, string User)
        {
            return loginService.GetUser(URLAPIConexion, User).GetAwaiter().GetResult();
        }

        public int Leido(int Idd)
        {
            return _service.Leido(Idd).GetAwaiter().GetResult();
        }

        public string ObtenerUsuario(string NameUser)
        {
            return _service.ObtenerUsuario(NameUser).GetAwaiter().GetResult();
        }

        public List<CXN_MESSENGER> ObtenerListaMensajes(string _anotherUser, string meUser, DateTime Fecha)
        {
            return _service.ObtenerListaMensajes(_anotherUser, meUser, Fecha).GetAwaiter().GetResult();
        }

        public List<string> ListaUsuariosActivos()
        {
            return loginService.ListaUsuariosActivos().GetAwaiter().GetResult();
        }

        public bool SendMessage(CXN_MESSENGER messenger)
        {
            return _service.SendMessage(messenger).GetAwaiter().GetResult();
        }

        public byte[] getAvatar(string men_Usuario)
        {
            return _service.getAvatar(men_Usuario).GetAwaiter().GetResult();    
        }
    }
}
