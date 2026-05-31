using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ITwilio
    {
        List<CXN_LINKSCORTOS> getUrlsGenerated(string Docunmento);
        void Insertar(CXN_LINKSCORTOS C);
    }
}
