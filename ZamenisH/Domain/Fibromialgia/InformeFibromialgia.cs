using System;

namespace Domain.Fibromialgia
{
    public class InformeFibromialgia
    {
        public string Paciente { get; set; }
        public string TDocumento { get; set; }
        public string NDocumento { get; set; }
        public string Mes { get; set; }
        public int Cantidad { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public byte[] Logo { get; set; }
        public string Servicio { get; set; }
        public string Aseguradora { get; set; }
        public string Telefono { get; set; }
        public string Diagnostico { get; set; }
        public string Correo { get; set; }
        public string Edad { get; set; }
        public string Inicio { get; set; }
        public string Sexo { get; set; }
    }
}
