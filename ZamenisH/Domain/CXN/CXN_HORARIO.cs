using System;

namespace Domain.CXN
{
    public class CXN_HORARIO : CXN_CIA
    {
        public string Hor_Estado { get; set; }
        public int Hor_Pac_Id { get; set; }
        public int Hor_Pac_Bod { get; set; }
        public string Hor_Pac_Tipo_Serv { get; set; }
        public string Hor_Pac_Id_Hora { get; set; }
        public DateTime Hor_Pac_Fecha { get; set; }
        public DateTime Hor_Pac_Fecha_Cita { get; set; }
        public DateTime Hor_Pac_Hora { get; set; }
        public DateTime Hor_Pac_Hora_Cita { get; set; }
        public DateTime Hor_Pac_Llegada { get; set; }
        public DateTime Hor_Pac_Atendido { get; set; }
        public int Hor_Pac_Cia { get; set; }
        public int Hor_Pac_Ase { get; set; }
        public string Hor_Pac_Cup { get; set; }
        public string Hor_Pac_UsrGraba { get; set; }
        public string Hor_Pac_Minutos { get; set; }
        public string Hor_Pac_Razon { get; set; }
        public string Hor_Pac_MCancela { get; set; }
        public string Hor_Pac_RCancela { get; set; }
        public string Hor_Pac_Inasistencia { get; set; }
        public string Hor_Imp_Age { get; set; }
        public string Hor_Autoriza { get; set; }
        public string Hor_Valida { get; set; }
        public string Hor_ValDerechos { get; set; }
        public string Hor_Pac_Sal { get; set; }
        public string Hor_Observacion { get; set; }
        public string Hor_RcCaja { get; set; }
        public DateTime Hor_Pac_Solicita { get; set; }
        public string Hor_Tipo_Paciente { get; set; }
        public string Hor_Pac_Modalidad { get; set; }
        public int Hor_Id { get; set; }
        public string Hor_Usr_Admisiona { get; set; }
        public string Hor_Usr_Cancela { get; set; }
        public string Hor_Regimen { get; set; }
        public string Hor_Vales { get; set; }
        public string Hor_RegAtn { get; set; }
        public int Hor_CantSesion { get; set; }
        public string Hor_IniciaSesion { get; set; }
        public string Hor_AdmOpnened { get; set; }
        public int Hor_BloqEspaces { get; set; }
        public string Hor_GrupoServicios { get; set; }
        public string HorTecnoSalud { get; set; }
        public string Hor_ConceptoRecaudo { get; set; }
        public string Hor_DocFEModerador { get; set; }
        public DateTime Hor_Pac_Hora_Salida { get; set; }    
        public string HorObservaTemp { get; set; }
        public string SALECONSULTA { get; set; }
        public string Hor_DocFEModeradorRes { get;set; }
        public string Hor_DocFEModeradorCUFE { get; set; }
        public CXN_PACIENTES DatosPaciente { get; set; }
        public DateTime Hor_DocFEModeradorFechaHora { get; set; }
        public string Hor_DocFEModeradorNumeracion{ get; set; }
        public string Hor_ArrastraHistoria { get; set; }
        public string FormaPago { get; set; }
        public bool? Hor_AvisoCurInicio { get; set; }
        public string Hor_Color { get; set; }
        public string Hor_UsrCruce { get; set; }
        public string Hor_Cruce { get; set; }
        public DateTime Hor_FechaCruce { get; set; }
    }

    

    public class otrosDatosPacienteHorario : CXN_HORARIO
    {
        public string Pac_Bonos { get; set; }
        public string Pac_TipoId { get; set; }
        public string Pac_IdNum { get; set; }
        public string Pac_PrimerN { get; set; }
        public string Pac_SegundoN { get; set; }
        public string Pac_PrimerA { get; set; }
        public string Pac_SegundoA { get; set; }
        public string Pac_Telefono { get; set; }
        public string Pac_TelefonoAux { get; set; }
        public string Pac_Email { get; set; }
        public string Pac_Dep_Cod { get; set; }
        public string Pac_Mun_Cod { get; set; }
        public string Bod_Responsable { get; set; }
        public DateTime Pac_FechaNto { get; set; }
        public string Pac_Sexo { get; set; }
        public string Pac_Zona { get; set; }
        public string Pac_Contrato { get; set; }
        public string Pac_PaisOrigen { get; set; }
        public string Pac_PaisResidencia { get; set; }
        public byte[] ExtraLogo { get; set; }
        public byte[] ExtraLogo2 { get; set; }
        public string Pac_Categoria { get; set; }
        public string Pac_ECivilLoadAdmition { get; set; }
        public string Pac_AcudienteLoadAdmition { get; set; }
        public string Pac_ParentescoLoadAdmition { get; set; }
        public string Pac_DireccionLoadAdmition { get; set; }
        public string Pac_TelefonoLoadAdmition { get; set; }
        public string Pac_CorreoLoadAdmition { get; set; }
        public string VIH { get; set; }
        public string Hepatitis { get; set; }
        public string Pac_Ocupacion { get; set; }   


        //FHIR
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string IdentificacionProfesional { get; set; }
    }

    public class ComplementoHorario : ComplementoRCCaja
    {
        public byte[] ImageSMS { get; set; }
        public byte[] ImageEmail { get; set; }
    }


}
