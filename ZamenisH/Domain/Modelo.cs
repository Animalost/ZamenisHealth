using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Domain
{
    public class ReportCMAN : CMAN { }
    public class ReportHCFI : HCFI { }
    public class ReportPSI : PSI { }
    public class ReportEVO : Evoluciones { }
    public class ReportJuntas : Juntas { }
    public class ReportTF : HCTF { }
    public class ReportTO : HCTO { }
    public class ReportEpicrisis : HistoriasMedicasReports { }
    public class PrntAgendas : AgendasRecepcionImprime { }
    public class FacturacionReports : ReportesRecepcion { public string TReport { get; set; } }

    public class TXTException
    {
        public string Formulario { get; set; }
        public DateTime FechaHora { get; set; }
        public string Usuario { get; set; }
        public string Error { get; set; }
        public string Metodo { get; set; }
    }

    public class Class_DetailDE : Modelo
    {
        public string Hoy { get; set; }
        public string Texto1 { get; set; }
        public string Gran_Tot { get; set; }
        public string Car_Adm_Id { get; set; }
        public string Nombres { get; set; }
        public string Car_Item { get; set; }
        public string Pac_TipoId { get; set; }
        public string Hor_ValDerechos { get; set; }
        public string Car_Val_Un { get; set; }
        public string Hor_Autoriza { get; set; }
        public string Car_Cant { get; set; }
        public string Car_Val_Tot { get; set; }
    }

    public class AgendasRecepcionImprime : Modelo
    {
        public string Hora_Cita { get; set; }
        public string Hor_Observacion { get; set; }
        public string Tipo_Detalle { get; set; }
        public DateTime fecha_rpt_citas { get; set; }
        public string Dia_Cita { get; set; }
        public string ESPE { get; set; }
        public DateTime HoraCita { get; set; }
        public string Estado_Cita { get; set; }
    }

    public class OrdenesC : Modelo
    {
        public string Com_Email { get; set; }
        public int Num_Orden { get; set; }
        public string Horario { get; set; }
        public string Recibe { get; set; }
        public string Usuario { get; set; }
        public string ObservacionG { get; set; }
        public string Cod_Item_Pro { get; set; }
        public string Cod_ItemI { get; set; }
        public string Item { get; set; }
        public string Observacion { get; set; }
        public int Cantidad { get; set; }

    }
    public class FormatosR : Modelo
    {
        public static string Combo { get; set; }
        public static string Text2 { get; set; }
        public static string Text3 { get; set; }
        public static string Text4 { get; set; }
        public static string Text5 { get; set; }
        public string Firmas { get; set; }
        public string Firmas2 { get; set; }
        public string Pie { get; set; }
        public string CiudadFecha { get; set; }
        public string Cuerpo { get; set; }
        public static string Serial { get; set; }
    }

    public class Modelo
    {
        //Generales
        public int Admision { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteIdentificacion { get; set; }
        public string PacienteTelefono { get; set; }
        public string PacienteDireccion { get; set; }
        public string PacienteAseguradora { get; set; }
        public string EmpresaNombre { get; set; }
        public string EmpresaTelefono { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaIdentificacion { get; set; }
        public string ProfesionalNombre { get; set; }
        public DateTime FechaBase { get; set; }
        public byte[] Logo { get; set; }
        public string Com_Direccion { get; set; }
    }

    public class HCMG : HistoriasMedicasReports
    {
        public string Pac_Acudiente { get; set; }
        public string Pac_Parentesco { get; set; }
        public string Pac_DireccionAcu { get; set; }
        public string Pac_TelefonoAcu { get; set; }
        public string Pac_CorreoAcu { get; set; }
        public string Pac_Email { get; set; }
        public string HC_Patologia { get; set; }
        public string HC_TipoLes { get; set; }
        public string HC_DescHer { get; set; }
        public string HC_ConsCant { get; set; }
        public string HC_TejCom { get; set; }
        public string HC_CaracTej { get; set; }
        public string HC_Exudado { get; set; }
        public string HC_SignosInf { get; set; }
        public string HC_PielCirc { get; set; }
        public string HC_Analisis { get; set; }
        public string HC_PManejo { get; set; }
        public string HC_PruebasDiag { get; set; }
        public string HC_Complicacion { get; set; }
        public string HC_Dolor { get; set; }
        public string HC_Estado { get; set; }
        public string HC_Vez { get; set; }
        public string Med_Largo { get; set; }
        public string Med_Ancho { get; set; }
        public string Med_Profundidad { get; set; }
        public string Med_Determinacion { get; set; }
        public string Med_Observacion { get; set; }
        public string HC_ServCatalogo { get; set; }
        public string HC_CupCatalogo { get; set; }

        //FHIR
        public string DX1Code { get; set; }
        public string DX2Code { get; set; }
        public string DX3Code { get; set; }
    }
    public class CMAN : HistoriasMedicasReports
    {
        public string Cam_ActualizaOb { get; set; }
        public string Cam_UsrActualiza { get; set; }
        public string Cam_Descripcion { get; set; }
        public string Adherencia { get; set; }
    }

    public class HCFI : HCMG
    {
        public string HC_RH { get; set; }
        public string HC_EAV { get; set; }
        public string HC_Ant { get; set; }
        public string HC_Mental { get; set; }
        public string HC_OrgSent { get; set; }
        public string HC_PerimetroC { get; set; }
        public string HC_Glasshow { get; set; }
        public string HC_Embriaguez { get; set; }
        public string HC_ObservaFis { get; set; }
        public string HC_ObservaNeu { get; set; }
        public string HC_Cabeza { get; set; }
        public string HC_Cuello { get; set; }
        public string HC_Torax { get; set; }
        public string HC_Pulmonar { get; set; }
        public string HC_Abdomen { get; set; }
        public string HC_Ombligo { get; set; }
        public string HC_Ano { get; set; }
        public string HC_Extremidades { get; set; }
        public string HC_Orl { get; set; }
        public string HC_Genitales { get; set; }
        public string HC_Egreso { get; set; }
        public string HC_PerimetroA { get; set; }
        public string HC_Talla { get; set; }
    }

    public class PSI : HistoriasMedicasReports
    {
        public string HC_Estado { get; set; }
        public string HC_Patologia { get; set; }
        public string HC_Fracturas { get; set; }
        public string HC_Tox_Ale { get; set; }
        public string HC_Riesgo { get; set; }
        public string HC_Psicologicos { get; set; }
        public string HC_Otros { get; set; }
        public string HC_FacPro { get; set; }
        public string HC_ImpDiag { get; set; }
        public string HC_Pronostico { get; set; }
        public string HC_Paccion { get; set; }
        public string HC_Recomienda { get; set; }
        public string HC_CIE10 { get; set; }
        public string HC_Esc_Dol { get; set; }
        public string HC_TEvolucion { get; set; }
        public string HC_Fisiatra { get; set; }
        public string HC_Psiquiatra { get; set; }
        public string HC_Remite { get; set; }
        public string HC_Caracteristica { get; set; }
        public string HC_RedApSoc { get; set; }
        public string HC_FSoporte { get; set; }
        public string HC_ObSoporte { get; set; }
        public string HC_OtroSoporte { get; set; }
        public string HC_TFamilia { get; set; }
        public string HC_TRelacion { get; set; }
        public string HC_EstadoOtro { get; set; }
        public string HC_TRelacion_2 { get; set; }
        public string HC_TiRelacion { get; set; }
        public string HC_Compo { get; set; }
        public string HC_Sust { get; set; }
        public string HC_ApGen_3 { get; set; }
        public string HC_Sust_2 { get; set; }
        public string HC_EstSex { get; set; }
        public string HC_ObservSex { get; set; }
        public string HC_Satis { get; set; }
        public string HC_DolMol { get; set; }
        public string HC_AutoEsq { get; set; }
        public string HC_ObseAuto { get; set; }
        public string HC_Suicida { get; set; }
        public string HC_LabEst { get; set; }
        public string HC_ObLav { get; set; }
        public string HC_TiLab { get; set; }
        public string HC_ApGen_1 { get; set; }
        public string HC_ApGen_2 { get; set; }
        public string HC_Sue_1 { get; set; }
        public string HC_Cons { get; set; }
        public string HC_ObCons { get; set; }
        public string HC_Aten_1 { get; set; }
        public string HC_ObAten { get; set; }
        public string HC_Aten_2 { get; set; }
        public string HC_Orien_1 { get; set; }
        public string HC_ObOrien { get; set; }
        public string HC_ObSue { get; set; }
        public string HC_Sue_2 { get; set; }
        public string HC_Sue_3 { get; set; }
        public string HC_Sue_4 { get; set; }
    }

    public class Evoluciones : HistoriasMedicasReports
    {
        public string Evo_Hora { get; set; }
        public string Evo_Sesion { get; set; }
        public string Evo_Evo { get; set; }
        public string Evo_Fase { get; set; }
        public string Evo_Texto { get; set; }
    }
    public class HCTO : Modelo
    {
        public string Edad { get; set; }
        public string Ocupacion { get; set; }
        public string RMedico { get; set; }
        public string MSD_MCP { get; set; }
        public string MSD_MCA { get; set; }
        public string MSD_MBP { get; set; }
        public string MSD_MBA { get; set; }
        public string MSD_MHP { get; set; }
        public string MSD_MHA { get; set; }
        public string MSD_MEP { get; set; }
        public string MSD_MEA { get; set; }
        public string MSD_MCIP { get; set; }
        public string MSD_MCIA { get; set; }
        public string MSD_MPP { get; set; }
        public string MSD_MPA { get; set; }
        public string MSD_MRP { get; set; }
        public string MSD_MRA { get; set; }
        public string MSD_MPIEP { get; set; }
        public string MSD_MPIEA { get; set; }

        public string MSI_MCP { get; set; }
        public string MSI_MCA { get; set; }
        public string MSI_MBP { get; set; }
        public string MSI_MBA { get; set; }
        public string MSI_MHP { get; set; }
        public string MSI_MHA { get; set; }
        public string MSI_MEP { get; set; }
        public string MSI_MEA { get; set; }
        public string MSI_MCIP { get; set; }
        public string MSI_MCIA { get; set; }
        public string MSI_MPP { get; set; }
        public string MSI_MPA { get; set; }
        public string MSI_MRP { get; set; }
        public string MSI_MRA { get; set; }
        public string MSI_MPIEP { get; set; }
        public string MSI_MPIEA { get; set; }

        public string MSD_AAP { get; set; }
        public string MSD_AADP { get; set; }
        public string MSD_AATP { get; set; }
        public string MSD_AALP { get; set; }
        public string MSD_AGAP { get; set; }
        public string MSD_AGACILP { get; set; }
        public string MSD_AGAESFP { get; set; }
        public string MSD_PINFP { get; set; }
        public string MSD_PINTP { get; set; }
        public string MSD_PLATP { get; set; }

        public string MSD_AAA { get; set; }
        public string MSD_AADA { get; set; }
        public string MSD_AATA { get; set; }
        public string MSD_AALA { get; set; }
        public string MSD_AGAA { get; set; }
        public string MSD_AGACILA { get; set; }
        public string MSD_AGAESFA { get; set; }
        public string MSD_PINFA { get; set; }
        public string MSD_PINTA { get; set; }
        public string MSD_PLATA { get; set; }

        public string MSI_AAP { get; set; }
        public string MSI_AADP { get; set; }
        public string MSI_AATP { get; set; }
        public string MSI_AALP { get; set; }
        public string MSI_AGAP { get; set; }
        public string MSI_AGACILP { get; set; }
        public string MSI_AGAESFP { get; set; }
        public string MSI_PINFP { get; set; }
        public string MSI_PINTP { get; set; }
        public string MSI_PLATP { get; set; }

        public string MSI_AAA { get; set; }
        public string MSI_AADA { get; set; }
        public string MSI_AATA { get; set; }
        public string MSI_AALA { get; set; }
        public string MSI_AGAA { get; set; }
        public string MSI_AGACILA { get; set; }
        public string MSI_AGAESFA { get; set; }
        public string MSI_PINFA { get; set; }
        public string MSI_PINTA { get; set; }
        public string MSI_PLATA { get; set; }

        public string HC_A_T1 { get; set; }
        public string HC_A_T2 { get; set; }
        public string HC_A_T3 { get; set; }
        public string HC_A_T4 { get; set; }
        public string HC_A_T5 { get; set; }
        public string HC_A_T6 { get; set; }
        public string HC_A_T7 { get; set; }
        public string HC_A_T8 { get; set; }
        public string HC_A_T9 { get; set; }
        public string HC_A_T10 { get; set; }

        public string HC_A_C1 { get; set; }
        public string HC_A_C2 { get; set; }
        public string HC_A_C3 { get; set; }
        public string HC_A_C4 { get; set; }
        public string HC_A_C5 { get; set; }
        public string HC_A_C6 { get; set; }
        public string HC_A_C7 { get; set; }
        public string HC_A_C8 { get; set; }
        public string HC_A_C9 { get; set; }
        public string HC_A_C10 { get; set; }
        public string HC_A_C11 { get; set; }
        public string HC_A_C12 { get; set; }
        public string HC_A_C13 { get; set; }
        public string HC_A_C14 { get; set; }
        public string HC_A_C15 { get; set; }
        public string HC_A_C16 { get; set; }
        public string HC_A_C17 { get; set; }
        public string HC_A_C18 { get; set; }
        public string HC_A_C19 { get; set; }
        public string HC_A_C20 { get; set; }
        public string HC_A_C21 { get; set; }
        public string HC_A_C22 { get; set; }
        public string HC_A_C23 { get; set; }
        public string HC_A_C24 { get; set; }

        public string HC_A_T11 { get; set; }
        public string HC_A_T12 { get; set; }
        public string HC_A_T13 { get; set; }
        public string HC_A_T14 { get; set; }
        public string HC_A_T15 { get; set; }
        public string HC_A_T16 { get; set; }
        public string HC_A_T17 { get; set; }
        public string HC_A_T18 { get; set; }

        public string HC_A_T19 { get; set; }
        public string HC_A_T20 { get; set; }
        public string HC_A_T21 { get; set; }
        public string HC_A_T22 { get; set; }
        public string HC_A_T23 { get; set; }
        public string HC_A_T24 { get; set; }

        public string HC_A_C25 { get; set; }
        public string HC_A_C26 { get; set; }
        public string HC_A_C27 { get; set; }
        public string HC_A_C28 { get; set; }
        public string HC_A_C29 { get; set; }

        public string HC_A_T25 { get; set; }
        public string HC_A_T26 { get; set; }
        public string HC_A_T27 { get; set; }
        public string HC_A_T28 { get; set; }
        public string HC_A_T29 { get; set; }
        public string HC_A_T30 { get; set; }
        public string HC_A_T31 { get; set; }
        public string HC_A_T32 { get; set; }
        public string HC_A_T33 { get; set; }
        public string HC_A_T34 { get; set; }
        public string HC_A_T35 { get; set; }

        public string HC_A_T37 { get; set; }
        public string HC_A_T38 { get; set; }
        public string HC_A_T36 { get; set; }
        public string HC_A_T39 { get; set; }

        public string HC_A_T40 { get; set; }
        public string HC_A_T41 { get; set; }
        public string HC_A_T42 { get; set; }
        public string HC_A_T43 { get; set; }

        public string HC_A_T48 { get; set; }
        public string HC_A_T47 { get; set; }
        public string HC_A_T45 { get; set; }
        public string HC_A_T44 { get; set; }
        public string HC_A_T50 { get; set; }
        public string HC_A_T51 { get; set; }
        public string HC_A_T52 { get; set; }
        public string HC_A_T53 { get; set; }
        public string HC_A_T54 { get; set; }
        public string HC_A_T55 { get; set; }
        public string HC_A_T56 { get; set; }
        public string HC_A_T57 { get; set; }
        public string HC_A_T58 { get; set; }
        public string HC_A_T59 { get; set; }
        public string HC_A_T60 { get; set; }
        public string HC_HisOcu { get; set; }
        public string HC_AntLaboral { get; set; }
        public string HC_ObservaFMS { get; set; }
        public string HC_A_ObservaVD { get; set; }
        public string HC_A_ObservaH { get; set; }
        public string HC_A_ObservaAVD { get; set; }
        public string HC_HabRut { get; set; }
        public string HC_HisFam { get; set; }
        public string HC_DiagMedico { get; set; }
        public string HC_DiagOcu { get; set; }
        public string HC_PronoOcu { get; set; }
        public string HC_Acciones { get; set; }
    }
    public class Juntas : HistoriasMedicasReports
    {
        public string Jun_Fisiatra { get; set; }
        public string Jun_TF { get; set; }
        public string Jun_TO { get; set; }
        public string Jun_Psicologo { get; set; }
        public string Jun_DX_FI { get; set; }
        public string Jun_DX_TF { get; set; }
        public string Jun_DX_TO { get; set; }
        public string Jun_DX_PS { get; set; }
        public string Jun_Pro_FI { get; set; }
        public string Jun_Pro_TF { get; set; }
        public string Jun_Pro_TO { get; set; }
        public string Jun_Pro_PS { get; set; }
        public string Jun_Con_FI { get; set; }
        public string Jun_Con_TF { get; set; }
        public string Jun_Con_TO { get; set; }
        public string Jun_Con_PS { get; set; }
        public string Jun_Observa { get; set; }
        public string Jun_Observa_TO { get; set; }
        public string Jun_Observa_PS { get; set; }
        public DateTime Jun_Prox_Cita { get; set; }
    }

    public class HCTF : HCMG
    {
        public string HC_Hijos { get; set; }
        public string HC_EstadoC { get; set; }
        public string HC_Estudios { get; set; }
        public string HC_TiempoE { get; set; }
        public string HC_ExaDiag { get; set; }
        public string HC_TratamientosP { get; set; }
        public string HC_DolorEIAN { get; set; }
        public string HC_EVA { get; set; }
        public string HC_CaracDol { get; set; }
        public string HC_Text60 { get; set; }
        public string HC_SensiDol { get; set; }
        public string HC_SintAso { get; set; }
        public string HC_HabTox { get; set; }
        public string HC_Cual1 { get; set; }
        public string HC_FrecDol { get; set; }
        public string HC_TipoDol { get; set; }
        public string HC_Cual2 { get; set; }
        public string HC_AntNeu { get; set; }
        public string HC_SalMen { get; set; }
        public string HC_OsteoMusc { get; set; }
        public string HC_Endocrino { get; set; }
        public string HC_Quir { get; set; }
        public string HC_Resp { get; set; }
        public string HC_Derma { get; set; }
        public string HC_Farma { get; set; }
        public string HisCabeza { get; set; }
        public string HisHombros { get; set; }
        public string HisBrazos { get; set; }
        public string HisCadera { get; set; }
        public string HisRodilla { get; set; }
        public string HisRotula { get; set; }
        public string HisTibia { get; set; }
        public string HisTobillo { get; set; }
        public string HisPie { get; set; }
        public string HisDiagnostico { get; set; }
        public string HisPronostico { get; set; }
        public string HisAcciones { get; set; }
        public string HisConducta { get; set; }
        public string Marcha_Mec { get; set; }
        public string HisCuello { get; set; }
        public string HisEspaldaA { get; set; }
        public string HisTorax { get; set; }
        public string HisAbdomen { get; set; }
        public string HisEspaldaB { get; set; }
        public string HisPelvis { get; set; }
        public string HisEscapula { get; set; }
        public string HisColumna { get; set; }
        public string HisCrestasIliacas { get; set; }
        public string HisGluteos { get; set; }
        public string HisFosa { get; set; }
        public string HC_CC1 { get; set; }
        public string HC_CC2 { get; set; }
        public string HC_CC3 { get; set; }
        public string HC_CC4 { get; set; }
        public string HC_CC5 { get; set; }
        public string HC_CC6 { get; set; }
        public string HC_CD1 { get; set; }
        public string HC_CD2 { get; set; }
        public string HC_CD3 { get; set; }
        public string HC_CD4 { get; set; }
        public string HC_H1 { get; set; }
        public string HC_H2 { get; set; }
        public string HC_H3 { get; set; }
        public string HC_H4 { get; set; }
        public string HC_H5 { get; set; }
        public string HC_H6 { get; set; }
        public string HC_C1 { get; set; }
        public string HC_C2 { get; set; }
        public string HC_C3 { get; set; }
        public string HC_C4 { get; set; }
        public string HC_C5 { get; set; }
        public string HC_C6 { get; set; }
        public string Hc_T1 { get; set; }
        public string Hc_T2 { get; set; }
        public string Hc_T3 { get; set; }
        public string Hc_T4 { get; set; }
        public string Hc_T5 { get; set; }
        public string Hc_T6 { get; set; }
        public string Hc_T7 { get; set; }
        public string Hc_T8 { get; set; }
        public string Hc_T9 { get; set; }
        public string Hc_T10 { get; set; }
        public string Hc_T11 { get; set; }
        public string Hc_T12 { get; set; }
        public string Hc_T13 { get; set; }
        public string Hc_T14 { get; set; }
        public string HisExtCui { get; set; }
        public string HisExtCuD { get; set; }
        public string HisEscalenosI { get; set; }
        public string HisEscalenosD { get; set; }
        public string HisTrapSupI { get; set; }
        public string HisTrapSupD { get; set; }
        public string HisTrapMedI { get; set; }
        public string HisTrapMedD { get; set; }
        public string HisTrapInfI { get; set; }
        public string HisTrapInfD { get; set; }
        public string HisSerAntI { get; set; }
        public string HisSerAntD { get; set; }
        public string HisPecMayI { get; set; }
        public string HisPecMayD { get; set; }
        public string HisRomI { get; set; }
        public string HisRomD { get; set; }
        public string HisAbSupI { get; set; }
        public string HisAbSupD { get; set; }
        public string HisAbdInfI { get; set; }
        public string HisAbdInfD { get; set; }
        public string HisOblicuoI { get; set; }
        public string HisOblicuoD { get; set; }
        public string HisExtDorsalI { get; set; }
        public string HisExtDorsalD { get; set; }
        public string HisExtLumbarI { get; set; }
        public string HisExtLumbarD { get; set; }
        public string HisGluMayI { get; set; }
        public string HisGluMayD { get; set; }
        public string HisGluMedI { get; set; }
        public string HisGluMedD { get; set; }
        public string HisCuadriI { get; set; }
        public string HisCuadriD { get; set; }
        public string HisIsquiI { get; set; }
        public string HisIsquiD { get; set; }
        public string HisGemeloI { get; set; }
        public string HisGemeloD { get; set; }
        public string HisTibAntI { get; set; }
        public string HisTibAntD { get; set; }
        public string HisEscaAntI { get; set; }
        public string HisEscaAntD { get; set; }
        public string HisEscaMedI { get; set; }
        public string HisEscaMedD { get; set; }
        public string HisEscaPosI { get; set; }
        public string HisEscaPosD { get; set; }
        public string HisECMI { get; set; }
        public string HisECMD { get; set; }
        public string HisCualLumI { get; set; }
        public string HisCualLumD { get; set; }
        public string HisCuadI { get; set; }
        public string HisCuadD { get; set; }
        public string HisIsquiI2 { get; set; }
        public string HisIsqui2D { get; set; }
        public string HisTensorI { get; set; }
        public string HisTensorD { get; set; }
        public string HisGastroI { get; set; }
        public string HisGastroD { get; set; }
        public string HisTAquilesI { get; set; }
        public string HisTAquilesD { get; set; }
    }

    public class EpicrisisReport : HistoriasMedicasReports
    {
        public DateTime HC_Vez { get; set; }        
        public string Pac_Acudiente { get; set; }
        public string Pac_Parentesco { get; set; }
        public string Pac_DireccionAcu { get; set; }
        public string Pac_TelefonoAcu { get; set; }
        public string Pac_CorreoAcu { get; set; }
        public string Pac_Email { get; set; }
        public string HC_Analisis { get; set; }
        public string HC_PManejo { get; set; }
        public Byte[] Code_QR { get; set; }
    }

    public class ReportNotas : Notas
    {
        public string CodigoEPSConvenio { get; set; }
        public string VIH { get; set; }
        public string Hepatitis { get; set; }
        public string NovedadHerida { get; set; }
        public List<CXN_NOTASMED> listaMedidas { get; set; }
    }

    public class Notas : HistoriasMedicasReports
    {
        public string Car_Cod { get; set; }
        public string Car_Item { get; set; }
        public int Car_Cant { get; set; }
        public string Car_Detalle { get; set; }
        public string Nota { get; set; }
        public string Observa { get; set; }
        public string Recomienda { get; set; }
        public string Adherencia { get; set; }
        public string CaracTej { get; set; }
    }

    public class HistoriasMedicasReports : Modelo
    {
        public string Diagnostico1 { get; set; }
        public string Diagnostico_Rel2 { get; set; }
        public string Diagnostico_Rel3 { get; set; }
        public string RegistroMedico { get; set; }
        public string Pac_Sexo { get; set; }
        public string Edad { get; set; }
        public DateTime FNto { get; set; }
        public DateTime HoraSalida { get; set; }
        public string Epidemia { get; set; }
        public string NotaAclaratoria { get; set; }
        public Byte[] Firma { get; set; }
        public string HC_MotivoC { get; set; }
        public string HC_EnfA { get; set; }
        public string HC_Neurologico { get; set; }
        public string HC_Cardiovascular { get; set; }
        public string HC_Gastrourinario { get; set; }
        public string HC_Osteomuscular { get; set; }
        public string HC_Gastrointestinal { get; set; }
        public string HC_Piel { get; set; }
        public string HC_Respiratorio { get; set; }
        public string HC_AntQui { get; set; }
        public string HC_AntFam { get; set; }
        public string HC_AntPat { get; set; }
        public string HC_AntFarma { get; set; }
        public string HC_AntAle { get; set; }
        public string HC_Hematolin { get; set; }
        public string HC_Presart { get; set; }
        public string HC_FreCar { get; set; }
        public string HC_FreRes { get; set; }
        public string HC_Temp { get; set; }
        public string HC_Peso { get; set; }
        public string HC_IMC { get; set; }
        public string HC_Altura { get; set; }
        public string HC_ITB { get; set; }
        public string HC_AparienciaG { get; set; }
        public string HC_EstadoEmo { get; set; }
        public string HC_EstadoNut { get; set; }
        public string HC_GradoC { get; set; }
        public string HC_ActEje { get; set; }
        public string Ocupacion { get; set; }

    }

    public class ListaNota
    {
        public string DX1 { get; set; }
        public string DX2 { get; set; }
        public string DX3 { get; set; }
        public Byte[] QRNota { get; set; }
    }

    public class ReportesRecepcion : Modelo
    {
        public int Recibo { get; set; }
        public int ValorReciboFactura { get; set; }
        public string Homologo { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public int TotalRpt { get; set; }
        public string Tipo { get; set; }
        public int Num_Cruce { get; set; }
        public string TReport { get; set; }

        public int Efectivo { get; set; }
        public int TarjetaCredito { get; set; }
        public int TarjetaDebito { get; set; }
        public int Nequi { get; set; }
        public int Daviplata { get; set; }
        public int OtrasBilleteras { get; set; }
        public int SinClasificar { get; set; }
    }

    public class FacturasR : Modelo
    {
        public string Letras { get; set; }
        public string Car_Cod { get; set; }
        public string Car_Item { get; set; }
        public int Cantidad { get; set; }
        public int Car_Val_Un { get; set; }
        public int Car_Val_Tot { get; set; }
        public int Fac_Num_Fac { get; set; }
        public int Total { get; set; }
        public string Ase_NitCia { get; set; }
        public string Ase_DVNitCia { get; set; }
        public string Ase_Telefono { get; set; }
        public DateTime Fac_Fecha_Des { get; set; }
        public DateTime Fac_Fecha_Has { get; set; }
        public string Fac_Num_Aut { get; set; }
        public string Fac_Res { get; set; }
        public string Fac_Observa { get; set; }
        public string Ase_Direccion { get; set; }
        public int Fac_Descuento { get; set; }
        public int Fac_Total { get; set; }
        public int Fac_Neto { get; set; }
        public string Usuario { get; set; }
        public byte[] Code_QR { get; set; }
        public byte[] Com_Logo { get; set; }

        //Doc Equivalente
        public string DocE_1 { get; set; }
        public string DocE_2 { get; set; }
        public string DocE_3 { get; set; }
        public string DocE_4 { get; set; }
        public int DocE_5 { get; set; }
        public int DocE_6 { get; set; }
        public DateTime DocE_7 { get; set; }
        public DateTime DocE_8 { get; set; }

        //Electronica
        public string Cufe { get; set; }
        public int VrCompartido { get; set; }
        public int Copago { get; set; }
        public int Anticipo { get; set; }
        public string CodPrestador { get; set; }
        public string ContratoPoliza { get; set; }
        public string Cobertura { get; set; }
        public string ModPago { get; set; }
        public byte[] QRCufe { get; set; }

        public int Dias { get; set; }
        public string MedioP { get; set; }
        public string MetodoP { get; set; }

        public string IVA { get; set; }
        public string ReteFuente { get; set; }
        public decimal ValorIVA { get; set; }
        public decimal ValorReteFuente { get; set; }
        public decimal ValorReteICA { get; set; }

        public string NombrePrestador { get; set; }

        public string PercentICA { get; set; }
        public string PercentFUENTE { get; set; }
        public int VrICA { get; set; }
        public int VrFUENTE { get; set; }
        public string LetrasImpuestos { get; set; }
        public int VrNeto { get; set; }
    }

    public class FacturacionRpt : Modelo
    {
        public int Descuento { get; set; }
        public string CodigoProd { get; set; }
        public string ItemProd { get; set; }
        public int VrUnitarioProd { get; set; }
        public string CantidadProd { get; set; }
        public int VrTotalProd { get; set; }
        public int VrTotalFac { get; set; }
        public int Deduccion { get; set; }
        public int VrNetoaPagar { get; set; }
        public string ValorLetras { get; set; }
        public string UsuarioFactura { get; set; }
        public string NumFac { get; set; }
        public string Resolucion { get; set; }
        public byte[] QRLogo { get; set; }
        public string CUFE { get; set; }
        public int Dcto { get; set; }

        public int Dias { get; set; }
        public string MedioP { get; set; }
        public string MetodoP { get; set; }
        public string Com_Resolucion_Electron { get; set; }
        public string PrefijoElectron { get; set; }
        public int NumElectron { get; set; }

        public DateTime Hora { get; set; }


        public string PercentICA { get; set; }
        public string PercentFUENTE { get; set; }
        public int VrICA { get; set; }
        public int VrFUENTE { get; set; }
        public string LetraImpuestos { get; set; }
        public int TotalImpuestos { get; set; }

    }

    public class Ordenes : Modelo
    {
        // General Servicios y Medicamentos
        public Byte[] QR { get; set; }
        public byte[] Firma { get; set; }
        public string Descripcion { get; set; }
        public string RegistroMedico { get; set; }
        public string OM_DX1 { get; set; }
        public string OM_DX2 { get; set; }
        public string OM_DX3 { get; set; }
        public string OM_DX1T { get; set; }
        public string OM_DX2T { get; set; }
        public string OM_DX3T { get; set; }
        public string Edad { get; set; }
        public string Genero { get; set; }
        public string OM_FHIR { get; set; }
        public int OM_Dias { get; set; }

        // Medicamentos
        public string OM_Duracion { get; set; }
        public string OM_Cantidad { get; set; }
        public string OM_Via { get; set; }
        public string OM_Medicamento { get; set; }
        public string OM_Presentacion { get; set; }
        public string OM_Detalle { get; set; }
        public string OM_Clasificacion { get; set; }
        public string OM_Tecnologia { get; set; }
        public int OM_Cada { get; set; }
        public string OM_Posologia { get; set; }
        public int OM_CantidadMedicamento { get; set; }        
    }
    
    public class RIPS_Class
    {
        public DateTime desdeRIPS { get; set; }
        public DateTime hastaRIPS { get; set; }
        public int aseRIPS { get; set; }
        public int ciaRIPS { get; set; }
        public string regimenRIPS { get; set; }
        public string claseRIPS { get; set; }
        public string tDocumentRIPS { get; set; }
        public string Numfac { get; set; }
        public string TipoInd { get; set; }
    }

    public class RIPS2275_2023Class
    {
        //Primera Parte
        public string numDocumentoIdObligado { get; set; }
        public string numFactura { get; set; }
        public string TipoNota { get; set; }
        public string numNota { get; set; }

        //Segunda Parte
        public string codMunicipioResidencia { get; set; }
        public string codPaisResidencia { get; set; }
        public string codSexo { get; set; }
        public string codZonaTerritorialResidencia { get; set; }
        public string consecutivo { get; set; }
        public string fechaNacimiento { get; set; }
        public string incapacidad { get; set; }
        public string numDocumentoIdentificacion { get; set; }
        public string tipoDocumentoIdentificacion { get; set; }
        public string numDocumentoldentificacion { get; set; }
        public string tipoUsuario { get; set; }
        public string codPaisOrigen { get; set; }

        //tercera parte - servicios
        public string causaMotivoAtencion { get; set; }
        public string codConsulta { get; set; }
        public string codDiagnosticoPrincipal { get; set; }
        public string codDiagnosticoRelacionado1 { get; set; }
        public string codDiagnosticoRelacionado2 { get; set; }
        public string codDiagnosticoRelacionado3 { get; set; }
        public string codPrestador { get; set; }
        public string codServicio { get; set; }
        //public string consecutivo { get; set; }
        public string fechalnicioAtencion { get; set; }
        public string finalidadTecnologiaSalud { get; set; }
        public string grupoServicios { get; set; }
        public string modalidadGrupoServicioTecSal { get; set; }
        public string numAutorizacion { get; set; }
        public string numDocumentoIdentificacion_P { get; set; }
        public string numFEVPagoModerador { get; set; }
        public string tipoDiagnosticoPrincipal { get; set; }
        public string tipoDocumentoIdentificacion_P { get; set; }
        public string tipoPagoModerador { get; set; }
        public string valorPagoModerador { get; set; }
        public string vrServicio { get; set; }
        public string conceptoRecaudo { get; set; }
        public string idMIPRES { get; set; }
        public string codProcedimiento { get; set; }
        public string viaIngresoServicioSalud { get; set; }
        public string codDiagnosticoRelacionado { get; set; }
        public string codComplicacion { get; set; }
        public string fechaSuministroTecnologia { get; set; }
        public string tipoOS { get; set; }
        public string codTecnologiaSalud { get; set; }
        public string nomTecnologiaSalud { get; set; }
        public int cantidadOS { get; set; }
        public string vrUnitOS { get; set; }
        public Dictionary<string, List<RIPS2275_2023Class>> DicDatosServicio { get; set; }

    }
}
