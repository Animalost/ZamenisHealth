using System;

namespace Domain.CXN
{
    public class CXN_CAMBIOSSOLICITADOS
    {
        public int Id { get; set; }
        public string CupComplejidad { get; set; }
        public string ServComplejidad { get; set; }
        public string Solicitante { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public int NumOrden { get; set; }
        public string Motivo { get; set; }
        public int AdmisionOfertante { get; set; }
    }
}
