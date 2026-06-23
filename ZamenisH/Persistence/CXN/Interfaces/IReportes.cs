using Domain.CXN;
using Domain;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IReportes
    {
        List<FirmasR> Firmas_Print(int Adm_Selected, FirmasR F, bool Autocompletar);
        List<GerencialR> Rpt_Atenciones(DateTime Desde, DateTime Hasta);
        void Rpt_Ing_Ger(DateTime Desde, DateTime Hasta);
        void Rpt_Rece_Ger(DateTime Desde, DateTime Hasta);
        List<ReportNotas> NotasMetodo(int Admision);
        List<HCMG> MedicinaGeneral(int Admision);
        List<HCMG> DiametrosHeridas(int Admision);
        List<ReportCMAN> CambiosManejo(int Admision);
        List<ReportHCFI> ReporteFisiatria(int Admition);
        List<ReportHCFI> ReportefisiatriaCompleto(int PacienteCom, DateTime Fecha, string user);
        List<ReportPSI> ReportePsicologia(int Admition);
        List<ReportEVO> ReporteEvoluciones(int Paciente,
                                                      int Admision,
                                                      string Tipo_Evo_Rep);
        List<ReportJuntas> ReporteJuntas(int Admition, string Tipo);
        List<ReportTF> ReporteTerapiaFisica(int Admition);
        List<ReportTO> ReporteTerapiaOcupacional(int Admition);
        Dictionary<int, List<ReportNotas>> NotasMetodoRDLC(int PacienteRDLC, DateTime Desde, DateTime Hasta);
        List<OrdenesC> ExportaOrdenCompra(int Orden);
        List<FacturacionReports> Exportar(DateTime Desde,
                                          DateTime Hasta,
                                          int Cia,
                                          int Ase,
                                          string Tipos);
        List<CXN_CIA> ActasMedicos(int Admision);
        List<CXN_HORARIO> BuscarHistorias(int Paciente, string Tipo);
        List<HCMG> MedicinaGeneralCompleto(int PacienteHCCompleto, DateTime Desde, DateTime Hasta, string userGenera);
        List<ReportHCFI> ReportefisiatriaCompleto(int PacienteCom, DateTime Desde, DateTime Hasta, string user);
        List<ReportEVO> ReporteEvoluciones(int Paciente,
                                                   DateTime Desde,
                                                   DateTime Hasta,
                                                   string Tipo_Evo_Rep);
        Dictionary<int, object> NotasMetodoRDLC(Dictionary<int, string> Dic);
        bool UpdateFormaPago(int FacZamenis, int Cia, string TipoPago, string Factura);
        List<CXN_HCRADIOLOGIA> RadiologiaReport(int Admision);
        List<CXN_HCRADIOLOGIA> ReporteRadiologiaCompleto(int PacienteCom, DateTime Desde, DateTime Hasta, string user);
        (Dictionary<int, string> DicNotas, Dictionary<int, string> DicHistorias) getAdmitionByInvoiceZamenis(int FacZamenis);
    }
}
