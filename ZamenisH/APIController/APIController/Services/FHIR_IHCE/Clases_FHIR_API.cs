namespace APIController.Services.FHIR_IHCE
{
    public class ReceiveToken
    {
        public string NitPrestador { get; set; }
        public string TenantID { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string URLToken { get; set; }
        public string Scope { get; set; }
    }
}
