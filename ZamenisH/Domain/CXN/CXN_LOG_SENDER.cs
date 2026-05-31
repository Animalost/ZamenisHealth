using System;

namespace Domain.CXN
{
    public class CXN_LOG_SENDER
    {
        public string Log_Tipo { get; set; }
        public DateTime Log_Fecha_Envio { get; set; }
        public string Log_Estado { get; set; }
        public string Log_Usuario { get; set; }
        public string Log_Mensaje { get; set; }
        public string Log_Destinatario { get; set; }
        public int Log_Admision { get; set; }
        public int Log_Id { get; set; }
    }
}
