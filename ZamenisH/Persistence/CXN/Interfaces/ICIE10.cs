using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ICIE10
    {
        List<CXN_CIE10> PorCodigo(string codigo);
        List<CXN_CIE10> PorDesc(string servicio);
        CXN_OM CargaDX(int Paciente);
        string BuscaDX(string CodDX);
        bool CreaCIE10(CXN_CIE10 C);
        bool UpdateCIE10(CXN_CIE10 C);
        List<CXN_CIE10> Ultimos(int Medico);
        CXN_OM CargaDXByAdmitionHCMG(int Admision);
        List<CXN_CUP> GetLista(string Servicio);
        string GetDXName(string Code);
    }
}
