using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IPayments
    {
        LinkPagos getStatusMonth();
        List<Pagos> getPagosHechos();
    }
}
