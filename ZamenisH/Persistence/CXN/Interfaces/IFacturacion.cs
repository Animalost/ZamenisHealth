using Domain.CXN;
using Domain;
using System.Collections.Generic;
using System;

namespace Persistence.CXN.Interfaces
{
    public interface IFacturacion
    {
        string enletras(string num);
        string insertarDocumento(CXN_FACTURA f);
        List<CXN_FACTURA> Filter(string Tipo, string TID, string ID, int Cia);
        List<FacturasR> Exporta_DOCE_IND_Orden(int Numero_DOCE_Ind, int cia, string Tipo);
        List<FacturasR> Fac_Export(int Numero_Fac, int cia, string Tipo);
        List<FacturasR> Fac_ExportOtrosServiciosTODOIMPUESTOS(int Numero_Fac, int cia, string Tipo, bool MostrarImpuestos);
        List<CXN_CARGOS> getCargosForInvoice(string Query);
        void changeTypeCargo(int posision, string Tipo);
        void excluirCargoFactura(int Posision);
        List<FacturasR> Exporta_DOCE_Orden(int Numero_DOCE, int cia, string Tipo);
        bool Graba_Factura_Orden(CXN_FACTURA F);
        bool Actualiza_Cargo_Facturado(CXN_FACTURA F, string AFacturar, bool EsGlobal);
        (List<CXN_CARGOS> lista, int ValorFac) FiltroFacturas(string Query);
        List<Class_DetailDE> getDetailDE(int Factura);
        bool Anula_Factura(CXN_FACTURA F);
        CXN_FACTURA consAutorizacion(string Numeero);
        bool existeHomologo(string Homologo, int Cia);
        void updateDatosGlosas(CXN_PAGOS F);
        CXN_FACTURA getFacElectronica(string Numeero);
        List<FacturasR> Fac_Export(string Numero_Fac, int cia);
        void UpdateCUV(string FE, string CUV);
        bool UpdateCUVManual(string FElectron, string CUV);
        List<CXN_FACTURA> GetFacturasForConvertElectron(int Cia, DateTime Desde, DateTime Hasta);
        CXN_FACTURA getFacturasTable(string Homologo, int Cia);
        bool InsertCopyTableFacturas(CXN_FACTURA F);
        List<CXN_CARGOS> getCargosTable(int FacZamenis, int Cia);
        void InsertCopyTableCargos(CXN_CARGOS C);
        int getTotalFac(int Cia, int Orden);
        bool UpdateFuenteICA(int fuente, int ica, int orden, int cia);
        List<int> GetAdmitionByType(string TipoCargo, int IdPaciente, DateTime Desde, DateTime Hasta, int Ase);
        int getValCuotasReceived(int FacZamenis, int Cia);
        List<int> getAdmitionsByFac(int FacZamenis, int Cia);
    }
}
