using System;
using System.Collections.Generic;

namespace Domain.CXN
{
    public class CXN_FACTURANC
    {
        public int Id { get; set; }
        public string FacturaElectronica { get; set; }
        public int OrdenPedido { get; set; }
        public DateTime FechaNC { get; set; }
        public DateTime HoraNC { get; set; }
        public string Cufe { get; set; }
        public string Resolucion { get; set; }
        public string NumeroNC { get; set; }
        public string Usuario { get; set; }
        public int Prestador { get; set; }
        public List<CXN_CARGOSNC> listaCargos { get; set; } 
    }
}
