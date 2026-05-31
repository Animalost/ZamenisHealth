using System.Collections.Generic;

namespace DocumentosElectronicos.Request
{
    public class RequestRecibidoFElectronDecodificado
    {
        public string TipoFactura { get; set; }
        public Factura Factura { get; set; }
        public string ClaveTecnica { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
    }
}
