using Domain.CONSUMOS;
using System.Collections.Generic;

namespace Persistence.CONSUMOS.Interfaces
{
    public interface IAsignacion
    {
        List<CON_ASIGNACION> GetBodegas();
        CON_ASIGNACION GetBodega(int NumerBod);
        CON_ASIGNACION GetBodega(string NameBod);
        bool CreaBodega(CON_ASIGNACION C);
        bool UpdateBodega(CON_ASIGNACION C);
    }
}
