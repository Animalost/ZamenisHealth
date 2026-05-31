using System;
using System.Collections.Generic;
using Domain.CXN;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface ITFGeneral
    {
        List<ClaseReportsFibro> ExportarInforme1TF(DatosForInforme I);
        List<CXN_HCTF> getAllTF(int Paciente, DateTime Desde, DateTime Hasta);
        bool updateTipoHistoria(int Admision, string TipoHistoria);
    }
}
