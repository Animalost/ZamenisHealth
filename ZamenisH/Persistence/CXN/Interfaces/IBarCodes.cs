using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IBarCodes
    {
        List<CXN_BARCODES> listaBarras();
        CXN_BARCODES GetCode(string CodeInterno, string CodeBar);
        bool Create(CXN_BARCODES C);
        bool Update(CXN_BARCODES C);
    }
}
