using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IHelisa
    {
        Dictionary<string, string> Claves(string nameClient, int Prestador);    
    }
}
