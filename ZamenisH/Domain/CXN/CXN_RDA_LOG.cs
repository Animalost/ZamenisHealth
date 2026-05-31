using System;

namespace Domain.CXN
{
    public class CXN_RDA_LOG
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; }
        public string Usuario { get; set; }
        public string Clase { get; set; }
    }
}
