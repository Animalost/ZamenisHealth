namespace APIController.Services.SignDocuments
{
    public class ContenidoDocumento
    {
        public string NombrePaciente { get; set; }
        public string Documento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string NombreDoctor { get; set; }
        public string RegistroDoctor { get; set; }
        public string FirmaDoctorBase64 { get; set; }
        public string TipoConsentimiento { get; set; }
        public string Compañia { get; set; }
        public string NIT { get; set; }
    }
}
