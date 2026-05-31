using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IMedidasHerida
    {
        List<CXN_HCMED> Carga_Med(int Admision);
        bool insertarHerida(CXN_HCMED H);
        void deleteMedida(int Posision);
    }
}
