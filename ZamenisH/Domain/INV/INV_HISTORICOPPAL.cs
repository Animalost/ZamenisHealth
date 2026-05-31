using System;

namespace Domain.INV
{
    public class INV_HISTORICOPPAL : ComplementoInvPpal
    {
        public int Id { get; set; }
        public string Lote { get; set; }
        public string Factura { get; set; }
        public int Cantidad { get; set; }
        public int Costo { get; set; }
        public int CodProducto { get; set; }
        public int IVA { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
    }

    public class ComplementoInvPpal
    {
        public string Nombre { get; set; }
        public string CodigoInterno { get; set; }
        public string CodigoProveedor { get; set; }
        public int Proveedor { get; set; }
        public int Prestador { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }
}
