using System.Collections.Generic;
using Domain.CXN;
using Domain;

namespace Persistence.CXN.Interfaces
{
    public interface INotasCuracion
    {
        void ActualizarServCuracion(CXN_CARGOS C, string User);
        CXN_NOTAS getLastNota(int Paciente);
        List<CXN_HORARIO> getHistorial(int Paciente);
        bool GrabarNota(CXN_NOTAS N);
        Dictionary<string, string> Diagnosticos(int Paciente);
        CXN_NOTAS seeNotaPrevReport(int Adm_Nota_Export);
        bool consNotaJefe(int Adm_Nota_Export);
        List<ReportNotas> NotasJefe(int Admision);
        List<CXN_CARGOS> CargoNota(int Adm_Nota_Export);
        void Estadisticas_Curaciones(CXN_ESTADISTICAS E);
        bool insertarHerida(CXN_NOTASMED H);
        List<CXN_NOTASMED> LoadHeridas(int Paciente, int Admision);
        List<CXN_NOTASMED> LoadHeridas(int Admision);
        List<CXN_NOTASMED> VerCitas(int Paciente);
        CXN_NOTASMED VerPosisionHerida(int Id);
        List<CXN_NOTASMED> LoadHistorialHeridas(int Paciente);
    }
}
