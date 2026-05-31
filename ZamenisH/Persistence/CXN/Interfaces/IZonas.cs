using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IZonas
    {
        string DepartamentoNombre(string CodDep);
        string DepartamentoCodigo(string NomDep);
        string MunicipioCodigo(string NomMun, string NomDep);
        string MunicipioNombre(string CodMun, string DepCod);
        List<CXN_ZONAS> _listadoCodigos(string DatoDep, string DatoMun, string Filtro);
    }
}
