using System;

namespace Domain.CXN
{
    public class ConsentimientosTemp : ComplementoDocsWEB
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string NombrePaciente { get; set; }
        public string Documento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string NombreDoctor { get; set; }
        public string RegistroDoctor { get; set; }
        public string FirmaDoctorBase64 { get; set; }
        public string TipoConsentimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string FirmaPacienteBase64 { get; set; }
        public string FechaFirma { get; set; }
        public string Seleccion1 { get; set; }
        public string Seleccion2 { get; set; }
        public string Seleccion3 { get; set; }
        public int Compañia { get; set; }
    }

    public class ComplementoDocsWEB
    {
        public byte[] FirmaDoc { get; set; }
        public byte[] FirmaPac { get; set; }
        public byte[] Logo { get; set; }
        public string Empresa { get; set; }
        public string TelEmpresa { get; set; }
        public string DirEmpresa { get; set; }
        public string NitEmpresa { get; set; }

    }
}
