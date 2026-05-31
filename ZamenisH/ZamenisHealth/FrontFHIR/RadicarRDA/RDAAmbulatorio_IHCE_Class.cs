using System;
using System.Collections.Generic;

namespace ZamenisHealth.FrontFHIR.RadicarRDA
{
    public class RDAAmbulatorio_IHCE_Class
    {
        //Datos Tecnicos
        public string Token { get; set; }
        //Datos Paciente
        public string TDocumento { get; set; }
        public string NDocumento { get; set; }
        public string OcupacionText { get; set; }
        public string CodePaisOrigen { get; set; }
        public string CodePaisResidencia { get; set; }
        public string NameEtnia { get; set; }
        public string NameDiscapacidad { get; set; }
        public string CodeSexoBiologico { get; set; }
        public string DisplaySexoBiologico { get; set; }
        public string DisplayCodeSexoBiologico { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string CodeDepartamento { get; set; }
        public string CodeMunicipio { get; set; }
        public string CodeZonaUrbana { get; set; }
        public string NameZonaUrbana { get; set; }
        public string CodeIdentidadGenero { get; set; }
        public string NameIdentidadGenero { get; set; }
        public DateTime FechaNto { get; set; }
        public DateTime HoraNto { get; set; }
        //Datos Prestador y Medico
        public string NombrePrestador { get; set; }        
        public string NDocumentoMedico { get; set; }
        public string TDocumentoMedico { get; set; }
        public string CodigoHabilitacion { get; set; }
        public string CodigoHabilitacionSucursal { get; set; }
        public string NitPrestador { get; set; }
        public string PrimerNombreMedico { get; set; }
        public string SegundoNombreMedico { get; set; }        
        public string PrimerApellidoMedico { get; set; }
        public string SegundoApellidoMedico { get; set; }
        //Datos Historia
        public DateTime FechaCita { get; set; }
        public DateTime HoraIngreso { get; set; }
        public DateTime HoraEgreso { get; set; }
        public string CodAseguradora { get; set; }
        public string DX1 { get; set; } // Code DX
        public string DX2 { get; set; } // Code DX
        public string DX3 { get; set; } // Code DX
        public string HistoriaBase64 { get; set; }
        //Encounter
        public string CodeEncounter { get; set; }
        public string DisplayEncounter { get; set; }
        public string CodeGrupoServicios { get; set; }
        public string NameGrupoServicios { get; set; }
        public string CodeREPS { get; set; }
        public string NameREPS { get; set; }
        public string CodeServicio { get; set; }
        public string NameServicio { get; set; }
        public string CodeConsultaExterna { get; set; }
        public string NameConsultaExterna { get; set; }
        public string CodeImpresionDiagnostica { get; set; }
        public string NameImpresionDiagnostica { get; set; }
        public string CodeEgreso { get; set; }
        public string NameEgreso { get; set; }
        //Aseguradora - Organization
        public string idOrganization { get; set; }
        public string nameOrganization { get; set; }
        //Listas
        public Incapacidad Incapacidades { get; set; } = new Incapacidad();
        public List<AlergiasInSite> AlergiasInSite { get; set; } = new List<AlergiasInSite>();
        public List<FactoresRiesgo> FactorRiesgo { get; set; } = new List<FactoresRiesgo>();
        public List<ServiciosOrdenadosInSite> ServiciosOrdenados { get; set; } = new List<ServiciosOrdenadosInSite>();
        public List<MedicamentosOrdenadosInSite> MedicamentosOrdenados { get; set; } = new List<MedicamentosOrdenadosInSite>();
    }


    public class AlergiasInSite
    {
        public string CodeFHIR { get; set; }
        public string Observacion { get; set; }
    }
    public class Incapacidad
    {
        public string TipoOrdenText { get; set; } // prorroga - nueva
        public string TipoOrdenCode { get; set; }
        public int Dias { get; set; }
    }
    public class FactoresRiesgo
    {
        public string Codigo { get; set; } 
        public string Descripcion { get; set; }
        public string Detalle { get; set; }
    }
    public class ServiciosOrdenadosInSite
    {
        public string CUP { get; set; }
        public string Descripcion { get; set; }
        public string ImpDiagnosticaCode { get; set; }
        public string ImpDiagnosticaText { get; set; }
        public DateTime FechaOrden { get; set; }
    }
    public class MedicamentosOrdenadosInSite
    {
        public string TipoTecnologiaText { get; set; }
        public string TipoTecnologiaCodigo { get; set; }
        public string MedicamentoCodigo { get; set; }
        public string MedicamentoDescripcion { get; set; }
        public string TecnologiaSaludCodigo { get; set; }
        public string TecnologiaSaludTexto { get; set; }
        public int DuracionTiempoDias { get; set; }
        public string DuracionTiempo { get; set; }
        public string DuracionTiempoTexto { get; set; }
        public string DuracionTiempoCodigo { get; set; }
        public string ViaCodigo { get; set; }
        public string ViaTexto { get; set; }
        public int Concentracion { get; set; }
        public string ConcentracionUnidadMedidaTexto { get; set; }
        public string ConcentracionUnidadMedidaCodigo { get; set; }
        public int DosificacionNumero { get; set; }
        public string DosificacionFrecuenciaTexto { get; set; }
        public string DosificacionFrecuenciaCodigo { get; set; }
    }
}
