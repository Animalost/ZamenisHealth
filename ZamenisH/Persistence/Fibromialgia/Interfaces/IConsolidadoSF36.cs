using System;
using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IConsolidadoSF36
    {
        List<E1Respuestas> getInformeRDL(DateTime Desde, DateTime Hasta);
    }
}
