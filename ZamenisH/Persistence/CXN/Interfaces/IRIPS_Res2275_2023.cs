using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IRIPS_Res2275_2023
    {
        string getCodeGrupoServicios(string NombreServicios);
        List<string> getTecSalud();
        void updateTecnoSalud(int Admision, string Valor);
        string getCodeTecnoSalud(string NombreServicios);
        string getNameTecnoSalud(string Code);
        string getNameTecnoSaludCExterna(int Code);
        string getCodeTecnoSaludCExterna(string NombreServicios);
        List<string> getTecSaludCEXTERNA();
        bool insertToken(string Token, int Cia);
        string getLastToken();
    }
}
