using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface INuevaAgenda
    {
        List<CXN_DISPONIBILIDAD_2> GetDisponibilidad(string Dia, int Med);
        List<CXN_HORARIO> GetHorariosSemana(DateTime Desde, DateTime Hasta, int Med, int Cia);
        CXN_HORARIO GetAdmision(int Admision);
    }
}
