using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IFirmasDigitales
    {
        bool InsertSign(CXN_FIRMASDIGITALES F);
        int InsertSign_Med(CXN_FIRMASDIGITALES_MED F);
        CXN_FIRMASDIGITALES_MED getFirmas_MED(int Posision);
        CXN_FIRMASDIGITALES getFirmas(int Admision);
        List<(int Adm, string Aut, int Cant)> getAdmitions(int PacienteID, int CIA, DateTime FechaLimite);
        List<(int Adm, string Aut, int Cant)> getAdmitionsByFacturacion(int PacienteID, int CIA, DateTime FechaLimite, DateTime Desde);
        string ImageNull();
        void EliminarFirma(int Admision);
        List<CXN_FIRMASDIGITALES_MED> getFirmas_MEDICAL(string Paciente);
    }
}
