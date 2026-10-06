using System;
using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IEncuestasXPaciente
    {
        List<ClaseReportsFibro> GetAllEncuestas(DateTime desde, DateTime hasta, string NumId);
        List<int> ObtenerEncuestaXPaciente(DateTime desde, DateTime hasta, string NumId);
        List<int> ObtenerEncuesta2XPaciente(DateTime desde, DateTime hasta, string NumId);
        ClaseReportsFibro getResultPacE1(int E1);
        FIB_ENCUESTA2 getResultE2(int E2);
        bool UpdateStatusEncuesta(int Pos, bool Status, string TEncuesta);
    }
}
