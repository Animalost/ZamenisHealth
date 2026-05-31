using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ICIerresCaja
    {
        Dictionary<string, int> getIngresos(int Code, DateTime Desde, DateTime Hasta);
        void ActualizarNumCruce(int NumCruce, int Cia, DateTime Desde, DateTime Hasta);
        void GrabarReporte(CXN_REPORTECAJA C);
        List<CXN_REPORTECAJA> GetReport(string consecutivo);
        List<CXN_REPORTECAJA> GetPrevios(int Cia, DateTime Desde, DateTime Hasta);
    }
}
