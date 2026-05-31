using System;

namespace Domain.CXN
{
    public class CXN_INVPPALCARGOS : CXN_INVPPALLISTA
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int IdListProd { get; set; }
        public string Usuario { get; set; }
        public int Cantidad { get; set; }
        public string Clase { get; set; }
        public int Prestador { get; set; }
        public string Observacion { get; set; }
    }
}
