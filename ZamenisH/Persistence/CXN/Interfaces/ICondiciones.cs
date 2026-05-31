using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ICondiciones
    {
        List<string> getCondiciones(int Pac);
        List<CXN_CONDICIONES> getCondiciones(int Pac, string Condicion);
        List<CXN_CONDICIONES> getCondicionesFHIR(int Pac);
        void updateHabilita(CXN_CONDICIONES c);
        void createCondiciones(CXN_CONDICIONES c);
        string getCondicionesForPrint(int Pac, DateTime Fecha);
        void createCondicionesInSite(CXN_ALERGIASINSITE c);
        List<CXN_ALERGIASINSITE> getCondicionesINSITE(int Admision);
        void deleteINSITE(int Id);
    }
}
