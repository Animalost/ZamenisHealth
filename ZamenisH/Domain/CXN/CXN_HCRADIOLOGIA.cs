using System;

namespace Domain.CXN
{
    public class CXN_HCRADIOLOGIA : ComplementoRadiologia
    {
        public int Id { get; set; }
        public int PacId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteTipoId { get; set; }
        public string PacienteId { get; set; }
        public string MotivoConsulta { get; set; }
        public string EnfermedadActual { get; set; }
        public string EvolucionSintomas { get; set; }
        public string AntecedentesRelevantes { get; set; }
        public string MedicamentosActuales { get; set; }
        public string AntecedentesRenales { get; set; }
        public string AntecedentesCardioVasculares { get; set; }
        public string Embarazo { get; set; }
        public string ImplantesMetalicos { get; set; }
        public string MenorEdad { get; set; }
        public string Presion { get; set; }
        public string Peso { get; set; }
        public string GlassHow { get; set; }
        public string Talla { get; set; }
        public string FResp { get; set; }
        public string FCar { get; set; }
        public string RH { get; set; }
        public string Conciencia { get; set; }
        public string IMC { get; set; }
        public string ObservacionExaMedico { get; set; }
        public string TipoEstudio { get; set; }
        public string MedioContraste { get; set; }
        public string ReaccionAdversa { get; set; }
        public string Tecnica { get; set; }
        public string Hallazgos { get; set; }
        public string DX1 { get; set; }
        public string DX2 { get; set; }
        public string DX3 { get; set; }
        public string NotaDX1 { get; set; }
        public string NotaDX2 { get; set; }
        public string NotaDX3 { get; set; }
        public string CausaExterna { get; set; }
        public string ImpDX1 { get; set; }
        public string ImpDX2 { get; set; }
        public string ImpDX3 { get; set; }
        public string EstudioComplementario { get; set; }
        public string ControlSeguimiento { get; set; }
        public int Compañia { get; set; }
        public int Aseguradora { get; set; }
        public int Medico { get; set; }
        public DateTime Fecha { get; set; }
        public int HCAdm { get; set; }
        public int HCCant { get; set; }
        public string NotaAclaratoria { get; set; }
    }
    public class ComplementoRadiologia : CXN_PACIENTES 
    { 
        public string Com_Nombre { get; set; }
        public string Com_Identificacion { get; set; }
        public string Com_Direccion { get; set; }
        public string Com_Telefono { get; set; }
        public string Bod_Responsable { get; set; }
        public string Bod_Reg_Med { get; set; }
        public byte[] Bod_Firma { get; set; }
        public string Ase_Descripcion { get; set; }
        public DateTime Hor_Pac_Hora_Salida { get; set; }
        public DateTime Hor_Pac_Hora_Cita { get; set; }
    } 
}
