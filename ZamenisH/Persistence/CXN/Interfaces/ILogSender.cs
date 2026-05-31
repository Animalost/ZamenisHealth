using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ILogSender
    {        
        void Log(int Admision,
                 string Salida,
                 string Mensaje_SMS,
                 string Celular,
                 string Usuario,
                 string TipoEnvio);

        void GrabaSQL_Evidencia(CXN_LOG_SENDER S);
        List<CXN_LOG_SENDER> Historial(DateTime Desde, DateTime Hasta);
        List<CXN_LOG_SENDER> Historial(DateTime Desde, DateTime Hasta, string Criterio);
    }
}
