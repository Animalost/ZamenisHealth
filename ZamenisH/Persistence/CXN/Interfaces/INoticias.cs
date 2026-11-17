using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface INoticias
    {
        List<CXN_NOTICIAS> GetAllNoticias();
        bool UpdateEstado(int C, bool Estado);
        bool CreaNoticia(CXN_NOTICIAS C);
    }
}
