using Domain;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IVentas
    {
        List<FacturacionRpt> Exp_Fac_Ven(int Docu_Ven, int cia, string Tipo);
        List<FacturacionRpt> Exp_Fac_Ven(string Docu_Ven, int cia);
        List<ReportesRecepcion> Rpt_RecibosdeCaja(DateTime Desde,
                                                         DateTime Hasta,
                                                         int Cia,
                                                         string Prestador,
                                                         bool Valores0);

        List<ReportesRecepcion> Rpt_FacturasVenta(DateTime Desde,
                                                  DateTime Hasta,
                                                  int Cia,
                                                  string Prestador,
                                                  string Tipo);

        bool Anula_Recepcion(CXN_VENTAS V);
        CXN_FACTURA getDAtosFactura(CXN_FACTURA F);
        List<CXN_FACTURA> ListaFacturasFacElectron(DateTime Desde, DateTime Hasta, int Compañia);
        List<FacturacionReports> Rpt_FacturasNC(DateTime Desde,
                                                 DateTime Hasta,
                                                 int Cia,
                                                 string Prestador,
                                                 string Tipo);
    }
}
