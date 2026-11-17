using System;

namespace Domain.CXN
{
    public class CXN_HISTORIAL_ADMISION 
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public string Gestion { get; set; }
        public CXN_HORARIO Horario { get; set; }
    }
}
