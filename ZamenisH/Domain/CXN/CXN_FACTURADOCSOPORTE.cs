using System;

namespace Domain.CXN
{
    public class CXN_FACTURADOCSOPORTE : CXN_CARGOSDOCSOPORTE
    {
        public int Id { get; set; }
        public string DocumentoElectronico { get; set; }
        public int DocumentoOrden { get; set; }
        public string UsuarioGenera { get; set; }
        public DateTime Fecha { get; set; }
        public decimal ReteFuente { get; set; }
        public decimal ReteICA { get; set; }
        public int CentroCosto { get; set; }
        public string Resolucion { get; set; }
        public string TextoResolucion { get; set; }
        public string CUFE { get; set; }
        public int IdCliente { get; set; }
    }
}
