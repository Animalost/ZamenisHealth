using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IPlantilla
    {
        List<CXN_PLANTILLA> getPlantillas(string user);
        bool savePlantillas(CXN_PLANTILLA P);
    }
}
