using Domain.CXN;

namespace Domain.Contabilidad
{
    public class DocumentoSoporte : CXN_FACTURADOCSOPORTE
    {
        public string CentroCostoName { get; set; }
        public string CentroCostoNit { get; set; }
        public string CentroCostoTel { get; set; }
        public string CentroCostoDir { get; set; }

        public string ClienteName { get; set; }
        public string ClienteDocumento { get; set; }
        public string ClienteDireccion { get; set; }
        public string ClienteCiudad { get; set; }
        public string ClienteEmail { get; set; }

        public string Documento { get; set; }
        public string Letras { get; set; }

        public byte[] CodeQR { get; set; }
        public byte[] LogoCia { get; set; }

        public decimal VrBruto { get; set; }
        public decimal VrReteFuente { get; set; }
        public decimal VrReteICA { get; set; }
        public decimal VrNeto { get; set; }
        public decimal VrSubtotal { get; set; }

        public string CUDS { get; set; }



        public string tipoDoc { get; set; }
    }
}
