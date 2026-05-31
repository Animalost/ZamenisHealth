using Domain.CXN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.CXN.Interfaces
{
    public interface IReportesPagos
    {
        Task<List<CXN_PAGOSASEGURADORAS>> ReportePagos(CXN_PAGOSASEGURADORAS parametros);
        List<CXN_ASEGURADORA> getAseguradoras();
        List<CXN_CIA> getAllCompañias();
        CXN_CIA getPrestadorbyName(string Name);
        CXN_ASEGURADORA getInfoFromAsebyName(string Name);
    }
}
