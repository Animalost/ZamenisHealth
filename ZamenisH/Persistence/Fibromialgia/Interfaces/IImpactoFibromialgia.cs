using System;
using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IImpactoFibromialgia
    {
        List<Preguntas3> ExportarImpacto(DateTime Desde,
                                        DateTime Hasta);
    }
}
