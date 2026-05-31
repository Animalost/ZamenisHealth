using System;

namespace Domain.CXN
{
    public class Inventario
    {
        public string Item { get; set; }
        public int Destino { get; set; }
        public string Origen { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public string Estado { get; set; }
        public int Id { get; set; }

    }
}
