using System;

namespace Domain.INV
{
    public class ReportsINV
    {
        public string Proveedor { get; set; }
        public string CodeInterno { get; set; }
        public string CodeProveedor { get; set; }
        public string Lote { get; set; }
        public string Factura { get; set; }
        public int Cantidad { get; set; }
        public int CostoUnitario { get; set; }
        public int CostoTotal { get; set; }
        public string ProductoName { get; set; }

        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string Usuario { get; set; }
        public string Prestador { get; set; }
        public string TipoReporte { get; set; }
    }
}
