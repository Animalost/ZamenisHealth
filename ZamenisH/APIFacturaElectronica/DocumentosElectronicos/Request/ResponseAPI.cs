namespace DocumentosElectronicos.Request
{
    public class ResponseAPI
    {
        public class XMLResponseF1
        {
            public string xml { get; set; }
            public string error { get; set; }
            public string StatusCode { get; set; }
            public string cufe { get; set; }




            public string qrdata { get; set; }
            public string[] DIAN { get; set; }
            public string clavtec { get; set; }
            public string id { get; set; }
            public string resForPDF { get; set; }
            public string resDifStatus { get; set; }
        }
    }
}
