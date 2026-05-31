using System;

namespace Domain.CXN
{
    public class CXN_TIPIFICADOR : ComplementoTipificador
    {
        public int Id { get; set; }
        public string NombreLlama { get; set; }
        public string TipoIdPaciente { get; set; }
        public string NumIdPaciente { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public int Aseguradora { get; set; }
        public string RazonLlamada { get; set; }
        public string Gestion { get; set; }
        public string Usuario { get; set; }
        public DateTime Hora { get; set; }
        public DateTime Fecha { get; set; }
        public string Ingreso { get; set; }
        public string NombrePaciente { get; set; }
    }

    public class ComplementoTipificador
    {
        public string AseguradoraName { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }
}
