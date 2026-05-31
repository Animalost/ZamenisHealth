using Domain.CXN;
using System.Collections.Generic;
using System;

namespace Persistence.CXN.Interfaces
{
    public interface IEncuestasSatis
    {
        bool getCantEncuestaCU(string Service, string Mes, int Año);
        bool getCantCitas(int Paciente, CXN_CONFENCUESTA C);
        bool getCantEncuestasPaciente(int Paciente, CXN_CONFENCUESTA C);
        bool GrabarNoRealizacionEncuesta(CXN_ENCUESTASATIS E);
        bool GrabarEncuesta(CXN_ENCUESTASATIS E);
        Dictionary<int, List<CXN_ENCUESTASATIS>> getEncuestas(DateTime Fecha, string TServ);
        Dictionary<int, List<CXN_ENCUESTASATIS>> getEncuestas(int Paciente, DateTime Fecha, string TServ);
        int getActual(CXN_CONFENCUESTA C);
        void updateActual(CXN_CONFENCUESTA C);
        List<CXN_ENCUESTASATIS> InformeMensual(DateTime Fecha, string Servicio, int Cia);
        CXN_ENCUESTASATIS getTrimestre(DateTime desde, DateTime hasta, string TServ, int Cia);
    }
}
