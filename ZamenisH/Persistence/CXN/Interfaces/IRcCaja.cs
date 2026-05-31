using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IRcCaja
    {
        void anularRcCaja(int Admision);
        List<RCCAJA> ReciboRpt(int Admition);
        List<RCCAJA> ReciboRpt(string Admition);
        int AgregarRecibo(CXN_RC_CAJA R);
        List<CXN_RC_CAJA> RCCAJAS(string TID, string ID);
        bool updateReciboHorario(int valor, int admision, string conRecaudo, string DocFE, string FPago);
        (int Valor, string Concepto) getValRcCaja(int Admision);
        bool EsConsulta(int Admision, string TipoServicio);
        List<CXN_PACIENTES> getRecibosFE(DateTime Fecha, int Cia);
        int getLastAdmition(int Admision);
    }
}
