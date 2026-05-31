using System;
using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IEncuestasXPaciente
    {
        List<ClaseReportsFibro> ExportarResGlobalPaciente(DateTime desde,
                                                        DateTime hasta,
                                                        string TipoId,
                                                        string NumId,
                                                        string usuario);
    }
}
