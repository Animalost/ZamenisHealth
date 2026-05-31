using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IMensajeria
    {
        Dictionary<string, string> getUsersforSendMessage();
        bool SendMessage(CXN_MESSENGER message);
        string getUsertoSendMessage(string Name);
        void Leido(int Id);
        CXN_MESSENGER Mensajes(string User);
        List<CXN_MESSENGER> getAllMessages(string _anotherUser, string meUser, DateTime Fecha);
    }
}
