using System;
using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IConsolidadoAIPEA
    {
        List<InformeFibromialgia> getEgresos(DateTime Desde, DateTime Hasta);
        List<InformeFibromialgia> getAdherencia(DateTime Desde, DateTime Hasta);
        List<InformeFibromialgia> getActivosYbase(DateTime Desde, DateTime Hasta);
        List<InformeFibromialgia> getPrevalentes(DateTime Desde, DateTime Hasta);
        List<InformeFibromialgia> getIngresos(DateTime Desde, DateTime Hasta);
    }
}
