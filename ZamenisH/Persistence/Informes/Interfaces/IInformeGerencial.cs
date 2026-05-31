using Domain.CXN;
using Domain.Informes;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Persistence.Informes.Interfaces
{
    public interface IInformeGerencial
    {
        Task<List<CXN_FACTURA>> getDatosFactura(InformeGerencia I);
        Task<List<CXN_FACTURA>> getDatosFacturaIndividual(InformeGerencia I);
        Task<List<CXN_FACTURA>> getDatosFacturaXPaciente(InformeGerencia I);
        Task<int> sumarValorXFactura(InformeGerencia I);
        Task<int> sumarValorTotalFacturasXPac(InformeGerencia I);
        Task<List<int>> getDatosAtendidosMES(InformeGerencia I);
        Task<BigInteger> sumarValorGlobal(InformeGerencia I);
    }
}
