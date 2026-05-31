using System;

namespace Domain.CXN
{
    public class CXN_CONDICIONES
    {
        public int Id { get; set; }
        public string Condicion { get; set; }
        public int Paciente { get; set; }
        public string Usuario { get; set; }
        public string Detalle { get; set; }
        public DateTime Fecha { get; set; }
        public string Habilita { get; set; }
        public string Excluye { get; set; }
        public DateTime? FechaExcluye { get; set; }
        public string CodigoFHIR { get; set; }
    }
}
