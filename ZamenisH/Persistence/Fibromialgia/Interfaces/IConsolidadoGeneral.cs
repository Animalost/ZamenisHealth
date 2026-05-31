using System;
using System.Collections.Generic;
using static Persistence.Fibromialgia.Metodos.MConsolidadoSF36Porcentual;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IConsolidadoGeneral
    {
        List<E1RespuestasPorcentual> generateReportRDLC(DateTime Desde, DateTime Hasta);
    }
}
