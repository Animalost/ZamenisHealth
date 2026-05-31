using System;
using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IEncuestaSatisfaccion
    {
        List<Preguntas3> ExportarResGlobalGeneral(DateTime desde, DateTime hasta);
    }
}
