using System;

namespace Domain.CXN
{
    public class CXN_HISTORYIA
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string Consulta { get; set; }
        public string Respuesta { get; set; }
        public string Usuario { get; set; }
        public string IA { get; set; }
    }
}
