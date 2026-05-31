using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IAseguradoras
    {
        CXN_ASEGURADORA getInfoFromAsebyCode(int Code);
        CXN_ASEGURADORA getInfoFromAsebyName(string Name);
        List<CXN_ASEGURADORA> getAseguradoras();
        List<string> CargarAseguradorasXServ(string TipoServ);
        bool updateAse(CXN_ASEGURADORA A);
        bool createAse(CXN_ASEGURADORA A);
    }
}
