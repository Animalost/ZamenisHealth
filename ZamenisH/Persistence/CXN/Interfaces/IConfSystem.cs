using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IConfSystem
    {
        bool updateImagenSystem(CXN_IMAGEN_SYSTEM I);
        List<CXN_IMAGEN_SYSTEM> getConfSystem();
    }
}
