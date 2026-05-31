using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IImagenes
    {
        List<CXN_HORARIO> getHistorias(string TID, string IDD);
        List<CXN_IMAGENES> getImagenesByAdmition(int Admision);
    }
}
