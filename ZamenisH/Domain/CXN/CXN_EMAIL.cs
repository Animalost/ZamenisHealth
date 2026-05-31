using System;

namespace Domain.CXN
{
    public class CXN_EMAIL
    {
        public string Ema_Email { get; set; }
        public string Ema_Pass { get; set; }
        public string Ema_Server { get; set; }
        public string Ema_Asunto { get; set; }
        public int Ema_Puerto { get; set; }
        public int Ema_Id { get; set; }
        public string Ema_Muestra { get; set; }
        public string Ema_URL_CitasM { get; set; }
        public string Ema_URL_CitasS { get; set; }
        public string Ema_Baja_Email { get; set; }
    }

    public class Recordatorios
    {
        public string Celular { get; set; }
        public int Admision { get; set; }
        public string TipoServicio { get; set; }
        public int Aseguradora { get; set; }
        public string Email { get; set; }
        public string URLCancelacionCitas { get; set; }
        public string URLBajaMensajes { get; set; }
        public string Paciente { get; set; }
        public DateTime HoraCita { get; set; }
        public DateTime FechaCita { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaTelefono { get; set; }
    }
}
