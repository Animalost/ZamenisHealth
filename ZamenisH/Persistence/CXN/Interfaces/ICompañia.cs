using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ICompañia
    {
        CXN_CIA getPrestadorbyCode(int Code);
        CXN_CIA getPrestadorbyName(string Name);
        List<CXN_CIA> getAllCompañias();
        bool ConsecutivoActualiza(int Cia, string TipoDoc, int NuevoCons);
        bool updateCompañia(CXN_CIA C);
        bool createCompañia(CXN_CIA C);
        void grabaLogo(CXN_CIA C);
        bool updateDataElectron(CXN_CIA C);
    }
}
