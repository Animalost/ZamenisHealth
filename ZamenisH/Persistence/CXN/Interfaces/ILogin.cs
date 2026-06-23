using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ILogin
    {
        CXN_LOGIN Loguear(string User, string Pass);
        string getClave(string User);
        bool changeClave(string User, string Clave);
        bool ValidarTipoUsuarioEspecialidad(string User, string Requiere);
        CXN_LOGIN getUser(string User);
        bool ActualizarFuncionario(CXN_LOGIN L);
        bool Graba_Funcionario(CXN_LOGIN L);
        bool resetClave(string User);
        List<CXN_LOGIN> getUsersforSendMessage();
        void saveAvatar(byte[] _avatar, string user);
        byte[] getAvatar(string User);
        bool AcceptTyC(string User);
        int ActualizarMisDatos(CXN_LOGIN L);
        bool updatePatron(string User, string Patron);
        CXN_LOGIN getDatosCode(int Code);
    }
}
