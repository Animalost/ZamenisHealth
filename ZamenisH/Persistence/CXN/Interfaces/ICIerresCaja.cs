using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ICIerresCaja
    {
        (Dictionary<string, int> Ventas, Dictionary<string, int> Caja, Dictionary<string, int> Particulares) getIngresos(int Code, DateTime Desde, DateTime Hasta);
        void ActualizarNumCruce(int NumCruce, int Cia, DateTime Desde, DateTime Hasta);
        int GrabarReporte2(CXN_REPORTECAJA2 C);
        List<CXN_REPORTECAJA2> GetReport2(string consecutivo, int Cia);
        List<CXN_REPORTECAJA2> GetPrevios(int Cia, DateTime Desde, DateTime Hasta);
    }
}
