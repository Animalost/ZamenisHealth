using System;

namespace Domain.CXN
{
    public class CXN_LINKSCORTOS
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public string Usuario { get; set; }
        public string Cliente { get; set; }
        public string Servidor { get; set; }
        public DateTime Fecha { get; set; }
        public ComplementoLinksCortos complemento { get; set; }
    }
    public class ComplementoLinksCortos
    {
        public string Estado { get; set; }
        public string Paciente { get; set; }
    }
}
