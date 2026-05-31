using System;
using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface ICManejo
    {
        bool insertarCambioManejo(CXN_CMAN C);
        List<CXN_CMAN> cargarCambiosPendientes();
        bool updateCambioManejo(CXN_CMAN C);
        int getIdPacByIdCMan(int IdPac);
        List<CXN_CMAN> getManejosxPaciente(int IdPac);
        (int IdHc, string NotaAclaratoria) getLastIdByIdPac(int IdPac);
        bool insertNotaAclaratoria(int IdHC, string texto);
        bool GrabarManejo(string Tipo,
                                   int Admision,
                                   string NotaNueva,
                                   string User);
        List<int> getManejosxPaciente(int IdPac, DateTime Desde, DateTime Hasta);
    }
}
