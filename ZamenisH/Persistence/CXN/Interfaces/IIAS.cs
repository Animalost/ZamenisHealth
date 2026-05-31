using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IIAS
    {
        bool Insertar(CXN_HISTORYIA H);
        List<CXN_HISTORYIA> Historial(string Usuario);
        CXN_HISTORYIA Historial(int id);
    }
}
