using System;

namespace Domain.CXN
{
    public class CXN_PAGOSASEGURADORAS
    {
        public  int Id { get; set; }
        public string Extracto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Factura { get; set; }
        public string Concepto { get; set; }
        public int Valor { get; set; }
        public int Prestador { get; set; }
        public int Aseguradora { get; set; }

        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string PrestadorName { get; set; }
        public string AseguradoraName { get; set; }
    }
}
