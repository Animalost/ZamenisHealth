using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IImpuestos
    {
        List<FUENTE2> getFuente();
        List<ICA2> getICA();
        int Fuente(int SubTotal, string FuenteTarifa);
        decimal ICA(string IcaTarifa);
    }
}
