using System;
using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IConsolidadoSF36Porcentual
    {
        List<CSF36_1> getInformeRDLC(DateTime Desde, DateTime Hasta);
    }
}
