using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IJuntas
    {
        List<CXN_HCJUNTAS> listarJuntasFirma(string TID, string IDD);
        void Firmar(int admision, string Med);
        bool InsertarJunta(CXN_HCJUNTAS H);
        List<CXN_HCJUNTAS> getJuntasForComplete(string TID, string NID, bool Informe);
        bool UpdateObservations(CXN_HCJUNTAS H);
        bool UpdateEgreso(int Admision, string Seleccion);
        void updateJuntaMedica(CXN_HCJUNTAS H);
        CXN_HCJUNTAS getJuntaCompleta(int Admision);
    }
}
