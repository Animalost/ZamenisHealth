using System;

namespace Domain.CXN
{
    public class CXN_REPORTECAJA2
    {
        public int IdRC { get; set; }
        public int Consecutivo { get; set; }
        public int Compañia { get; set; }
        public string Usuario { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public DateTime Generacion { get; set; }
        public string Tipo { get; set; }
        public string Clase { get; set; }
        public int Valor { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }

        public string ObservacionGeneral { get; set; }
    }
}
