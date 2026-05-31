using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface INotasAclaratorias
    {
        List<CXN_HORARIO> NotasMedicas(string TID, string NID, string Tipo);
    }
}
