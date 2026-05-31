using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IAntecedentesGlobales
    {
        void InsertarAntecedente(CXN_ANTECEDENTESFAMILIARES A);
        bool BuscaAntecedente(int Paciente, string CIE10, string Parentesco);
        List<CXN_ANTECEDENTESFAMILIARES> ObtenerAntecedentes(int Paciente);
        void EliminarAntecedente(int Id);
    }
}
