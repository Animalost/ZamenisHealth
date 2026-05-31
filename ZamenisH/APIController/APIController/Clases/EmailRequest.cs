namespace APIController.Clases
{
    public class EmailRequest
    {
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailPassword { get; set; }
        public string EmailBcc1 { get; set; }
        public string EmailBcc2 { get; set; }
        public string Asunto { get; set; }
        public string AttachmentFile { get; set; } //base64
        public string BodyMessage { get; set; }
        public string TipoArchivo { get; set; }
    }
}
