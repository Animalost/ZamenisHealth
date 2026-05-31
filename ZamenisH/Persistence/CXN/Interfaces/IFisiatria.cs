using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IFisiatria
    {
        bool ActualizaHCFI(CXN_HCFI HC);
        bool GrabaHCFI(CXN_HCFI HC);
        CXN_HCFI getLastHistory(int Paciente, DateTime Fecha);
        CXN_HCFI restoreHistory(int Admision);
        List<CXN_HCMG> ListaUltimasCitas(int Paciente, DateTime Fecha, int Medico);
    }
}
