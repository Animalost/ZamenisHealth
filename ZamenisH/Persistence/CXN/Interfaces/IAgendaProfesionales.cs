using System;
using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IAgendaProfesionales
    {
        string getEstadoCita(int Admision);
        List<CXN_HORARIO> Carga_Agenda(CXN_HORARIO H);
        string getEnfCitaMG(int Pac, DateTime Fecha);
    }
}
