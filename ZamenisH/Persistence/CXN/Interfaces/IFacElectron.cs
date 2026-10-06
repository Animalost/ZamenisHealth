using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Persistence.CXN.Interfaces
{
    public interface IFacElectron
    {
        List<CXN_MEDIOSPAGO> ListaMediosPago();
        string insertToken(string Token, int Prestador);
        string GetTokenSaved(int Cia);
        string insertNC(CXN_FACTURANC F);
        int GetFacZam(int Cia, string Homologo, string Tipo);
        List<CXN_FACTURANC> GetNotasCredito(string Tipo, DateTime Desde, DateTime Hasta, int Prestador);
        CXN_FACTURA GetFacZam(int Cia, int docZam);
        bool UpdateDatosDIAN(CXN_FACTURA F);
        string GetHomologo(int Cia, int docZam);
        string GetHomologoRcCaja(int Cia, int docZam);
        string GetHomologoVentas(int Cia, int docZam);
        Task<DataTable> getReportContableCompleto(int compania,
                                            string estado,
                                            DateTime desde,
                                            DateTime hasta);
    }
}
