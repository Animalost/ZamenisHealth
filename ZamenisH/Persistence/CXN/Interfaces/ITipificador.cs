using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ITipificador
    {
        List<CXN_TIPIFICADORRAZON> ListaRazones();
        bool Crea(CXN_TIPIFICADOR A);
        List<CXN_TIPIFICADOR> Reporte(DateTime Desde, DateTime Hasta);
    }
}
