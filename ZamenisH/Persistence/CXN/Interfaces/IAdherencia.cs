using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IAdherencia
    {
        string DescripcionAposito(string Aposito);
        List<CXN_ADHERENCIA> listaApositos();
        bool Crea(CXN_ADHERENCIA A);
        bool Actualiza(CXN_ADHERENCIA A);
    }
}
