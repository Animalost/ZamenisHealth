using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ICuentas
    {
        List<CXN_CUENTAS> getCuentas();
    }
}
