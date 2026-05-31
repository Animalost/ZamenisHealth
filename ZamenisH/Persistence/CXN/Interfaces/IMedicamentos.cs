using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IMedicamentos
    {
        List<CXN_MEDICAMENTOS> PorDesc(string Med);
    }
}
