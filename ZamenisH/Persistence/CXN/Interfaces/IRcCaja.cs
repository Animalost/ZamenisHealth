using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IRcCaja
    {
        List<RCCAJA> ReciboRpt(int Admition);
        List<RCCAJA> ReciboRpt(string Admition);
        int AgregarRecibo(CXN_RC_CAJA R);
        List<CXN_RC_CAJA> RCCAJAS(string TID, string ID);
        (int Valor, string Concepto) getValRcCaja(int Admision);
        bool EsConsulta(int Admision, string TipoServicio);
        int getLastAdmition(int Admision);
    }
}
