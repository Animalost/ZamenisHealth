using System;
using System.Collections.Generic;

namespace ZamenisHealth.FrontFHIR.RadicarRDA
{
    public class ClassRDAPaciente
    {
        //Datos Acceso
        public string Token { get; set; }
        //Datos Paciente
        public string TipoDocumentoPaciente { get; set; } // CC, RC, etc...
        public string NumeroDocumentoPaciente { get; set; }
        public string SexoBiologico { get; set; } // male - female   
        public string CodeIdentidadGenero { get; set; }
        public string CodPaisNacimiento { get; set; }
        public string CodPaisResidencia { get; set; }
        public string ZonaUrbana { get; set; } // U - R
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string CodeDepartamento { get; set; }
        public string CodeMunicipio { get; set; }
        public int CodEtnia { get; set; }
        public string CodDiscapacidad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime HoraNacimiento { get; set; }
        //Datos Cita
        public DateTime FechaAtencion { get; set; }
        public DateTime HoraIngreso { get; set; }
        public DateTime HoraSalida { get; set; }
        public string ModalidadAtencion { get; set; }
        public string GrupoServicios { get; set; }
        //Datos Prestador
        public string CodHabilitacionPrestador { get; set; }
        public string NitPrestador { get; set; }
        public string DVPrestador { get; set; }
        //Datos Profesional
        public string IdentificacionProfesional { get; set; }
        public string TipoIdentificacionProfesional { get; set; }
        //Antecedentes
        public AntecedentePaciente antecedentesPaciente { get; set; }
        public List<AntecedentesFamiliares> antecedentesFamiliares { get; set; }
    }

    public class AntecedentePaciente
    {
        public List<AntPatologico> Patologicos { get; set; } = new List<AntPatologico>();
        public List<AntPatologico> Farmacologicos { get; set; } = new List<AntPatologico>();
        public List<AntPatologico> Alergias { get; set; } = new List<AntPatologico>();
    }
    public class AntPatologico
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Detalle { get; set; }
    }
    public class AntecedentesFamiliares
    {
        public string Parentesco { get; set; }
        public string CIECod { get; set; }
        public string CieDesc { get; set; }
    }
}
