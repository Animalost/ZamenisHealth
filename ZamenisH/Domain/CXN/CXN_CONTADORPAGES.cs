using System;

namespace Domain.CXN
{
    public class CXN_CONTADORPAGES
    {
        public int Id { get; set; }
        public string Pagina { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public int Contador { get; set; }
    }
}
