using System;

namespace Domain.CXN
{
    public class CXN_PAGOS : CXN_FACTURA
    {
        public DateTime FRadica { get; set; }
        public int VrPagado { get; set; }
        public DateTime FPago { get; set; }
        public string RT { get; set; }
        public string Homologo { get; set; }
    }
}
