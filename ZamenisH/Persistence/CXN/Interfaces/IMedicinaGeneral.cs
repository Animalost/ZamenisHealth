using System;
using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IMedicinaGeneral
    {
        CXN_HCMG getIngresadoTemporal(int Admision);
        CXN_HCMG getLastHistory(int Paciente, DateTime Fecha);
        bool ActualizaHCMG(CXN_HCMG H);
        bool GrabaHCMG(CXN_HCMG H);
        List<CXN_HCMG> ResumenHCMGNotas(int Paciente);
        CXN_HCMG getResumen(int Admision);
        CXN_HCMG getLastHistoryToCopy(int Paciente);
        List<CXN_HCMG> ListaUltimasCitas(int Paciente, DateTime Fecha, int Medico);
    }
}
